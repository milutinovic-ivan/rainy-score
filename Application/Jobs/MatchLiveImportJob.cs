using Application.Intefraces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Diagnostics;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class MatchLiveImportJob : IJob
    {
        private readonly IMatchLiveService _matchLiveService;
        private readonly ILogger<MatchLiveImportJob> _logger;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<League> _leagueRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;
        private readonly IRepository<LeagueExternalMap> _leagueExternalMapRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MatchLiveImportJob(IMatchLiveService matchLiveService, ILogger<MatchLiveImportJob> logger,
           IRepository<Country> countryRepository,
           IRepository<League> leagueRepository,
           IRepository<Team> teamRepository,
           IRepository<MatchDetails> matchDetailsRepository,
           IRepository<LeagueExternalMap> leagueExternalMapRepository,
           IUnitOfWork unitOfWork)
        {
            _matchLiveService = matchLiveService;
            _logger = logger;
            _countryRepository = countryRepository;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _matchDetailsRepository = matchDetailsRepository;
            _leagueExternalMapRepository = leagueExternalMapRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;

            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                _logger.LogInformation("Job started");

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                //TODO
                //1. send data from job somehow, I will need to run job for yesterday and for today, FIX THIS
                var runDate = DateOnly.FromDateTime(DateTime.Today);

                //call service implementation to get list of match details
                var matchDetailsDataList = await _matchLiveService.GetMatchDetailsListAsync(runDate);

                //get all existing data from db
                var allTeams = await _teamRepository.GetAllAsync();
                var allContries = await _countryRepository.GetAllAsync();
                var allleagueExternalMaps = await _leagueExternalMapRepository.GetAllAsync(lem => lem.Include(lem => lem.League));
                var runDateMatchDetailsList = await _matchDetailsRepository.GetAllAsync(md => md.Where(m => m.MatchDate == runDate));

                //put results in dicts to not query db every time
                var teamsCache = allTeams.ToDictionary(t => t.Name.Trim(), t => t, StringComparer.OrdinalIgnoreCase);
                var countryCache = allContries.ToDictionary(c => c.Name.Trim(), c => c, StringComparer.OrdinalIgnoreCase);
                var leagueExternalMapCache = allleagueExternalMaps.ToDictionary(l => (l.DataSource, l.ExternalLeagueId), l => l);
                var matchDetailsCache = runDateMatchDetailsList.Where(md => md.FixtureId != null).ToDictionary(md => md.FixtureId!.Value, md => md);

                int updatedMatches = 0;
                int insertedMatches = 0;

                foreach (var matchDetailsData in matchDetailsDataList)
                {
                    //match is skipped if there is no league data or competition is cup
                    if (matchDetailsData.FixtureId is null || matchDetailsData.DataSource is null || matchDetailsData.ExternalLeagueId is null)
                    {
                        _logger.LogWarning($"Match skipped... No FixtureId or DataSource or ExternalLeagueId, " +
                            $"FixtureId: {matchDetailsData.FixtureId} ; " +
                            $"DataSource: {matchDetailsData.DataSource} ; " +
                            $"ExternalLeagueId: {matchDetailsData.ExternalLeagueId}");

                        continue;
                    }

                    //if match is finished but there is no full time remove match if exists and skip
                    if (matchDetailsData.Status == "FT" && (matchDetailsData.FullTimeHomeGoals is null || matchDetailsData.FullTimeAwayGoals is null))
                    {
                        if(matchDetailsCache.TryGetValue(matchDetailsData.FixtureId.Value, out var matchToDelete))
                        {
                            _matchDetailsRepository.Delete(matchToDelete);
                            _logger.LogInformation($"Match deleted... match is finished but there is no full time result");

                            continue;
                        }

                        _logger.LogInformation($"Match skipped... match is finished but there is no full time result");
                        continue;
                    }

                    //countries
                    var normalizedCountryName = string.IsNullOrWhiteSpace(matchDetailsData.Country)
                        ? "Unknown"
                        : matchDetailsData.Country.Trim();

                    if (!countryCache.TryGetValue(normalizedCountryName, out var country))
                    {
                        country = new Country
                        {
                            Name = normalizedCountryName
                        };

                        await _countryRepository.AddAsync(country);

                        countryCache.Add(normalizedCountryName, country);

                        _logger.LogInformation($"Country added... Name: {country.Name}");
                    }


                    //leagues
                    var leagueExternalMapKey = (matchDetailsData.DataSource, matchDetailsData.ExternalLeagueId.Value);

                    if (!leagueExternalMapCache.TryGetValue(leagueExternalMapKey, out var leagueExternalMap))
                    {
                        //insert new League
                        var league = new League();

                        //if service is implemented get league data from implementation
                        if (_matchLiveService is ILeagueService leagueService)
                        {
                            var leagueData = await leagueService.GetLeagueDataAsync(matchDetailsData.ExternalLeagueId.Value);

                            if (leagueData is null)
                            {
                                _logger.LogWarning($"Match skipped... LeagueService returned null");
                                continue;
                            }

                            league.Name = leagueData!.Name;
                            league.Country = country;
                            league.IsCup = leagueData.IsCup;
                        }
                        else
                        {
                            league.Name = matchDetailsData.LeagueName!;
                            league.Country = country;
                            league.IsCup = matchDetailsData.IsCup ?? false;
                        }

                        await _leagueRepository.AddAsync(league);

                        _logger.LogInformation($"League added... Name: {league.Name}");

                        //insert new LeagueExternalMap
                        leagueExternalMap = new LeagueExternalMap
                        {
                            League = league,
                            DataSource = matchDetailsData.DataSource,
                            ExternalLeagueId = matchDetailsData.ExternalLeagueId.Value
                        };

                        await _leagueExternalMapRepository.AddAsync(leagueExternalMap);

                        //add new LeagueExternalMap in cache dict
                        leagueExternalMapCache.Add(leagueExternalMapKey, leagueExternalMap);
                    }

                    //league is cup, skip match
                    if (leagueExternalMap.League.IsCup)
                    {
                        _logger.LogInformation($"Match skipped... it is cup not league");
                        continue;
                    }


                    //add home team just if already not exists
                    var homeTeamName = matchDetailsData.HomeTeam.Trim();
                    if (!teamsCache.TryGetValue(homeTeamName, out var homeTeam))
                    {
                        homeTeam = new Team
                        {
                            Name = homeTeamName,
                            StadiumId = null
                        };

                        await _teamRepository.AddAsync(homeTeam);

                        //add new item in dict
                        teamsCache.Add(homeTeam.Name, homeTeam);

                        _logger.LogInformation($"Team added... Name: {homeTeam.Name}");
                    }

                    //add away team just if already not exists
                    var awayTeamName = matchDetailsData.AwayTeam.Trim();
                    if (!teamsCache.TryGetValue(awayTeamName, out var awayTeam))
                    {
                        awayTeam = new Team
                        {
                            Name = awayTeamName,
                            StadiumId = null
                        };

                        await _teamRepository.AddAsync(awayTeam);

                        //add new item in dict
                        teamsCache.Add(awayTeam.Name, awayTeam);

                        _logger.LogInformation($"Team added... Name: {awayTeam.Name}");
                    }

                    //MatchDetails

                    if(matchDetailsCache.TryGetValue(matchDetailsData.FixtureId.Value, out var existingMatch))
                    {
                        existingMatch.FullTimeHomeGoals = matchDetailsData.FullTimeHomeGoals;
                        existingMatch.FullTimeAwayGoals = matchDetailsData.FullTimeAwayGoals;
                        existingMatch.FullTimeWiner = matchDetailsData.FullTimeWiner;
                        existingMatch.HalfTimeHomeGoals = matchDetailsData.HalfTimeHomeGoals;
                        existingMatch.HalfTimeAwayGoals = matchDetailsData.HalfTimeAwayGoals;
                        existingMatch.HalfTimeWiner = matchDetailsData.HalfTimeWiner;
                        existingMatch.Status = matchDetailsData.Status;

                        updatedMatches++;
                    }
                    else
                    {
                        var matchDetails = new MatchDetails
                        {
                            League = leagueExternalMap.League,
                            Season = matchDetailsData.MatchDate.Year,
                            MatchDate = matchDetailsData.MatchDate,
                            MatchTime = matchDetailsData.MatchTime,
                            HomeTeam = homeTeam,
                            AwayTeam = awayTeam,
                            FullTimeHomeGoals = matchDetailsData.FullTimeHomeGoals,
                            FullTimeAwayGoals = matchDetailsData.FullTimeAwayGoals,
                            FullTimeWiner = matchDetailsData.FullTimeWiner,
                            HalfTimeHomeGoals = matchDetailsData.HalfTimeHomeGoals,
                            HalfTimeAwayGoals = matchDetailsData.HalfTimeAwayGoals,
                            HalfTimeWiner = matchDetailsData.HalfTimeWiner,
                            HomeWinOdds = matchDetailsData.HomeWinOdds,
                            DrawWinOdds = matchDetailsData.DrawWinOdds,
                            AwayWinOdds = matchDetailsData.AwayWinOdds,
                            GoalsOver25Odds = matchDetailsData.GoalsOver25Odds,
                            GoalsUnder25Odds = matchDetailsData.GoalsUnder25Odds,
                            IsHistory = matchDetailsData.IsHistory,
                            DataSource = matchDetailsData.DataSource,
                            FixtureId = matchDetailsData.FixtureId,
                            Status = matchDetailsData.Status
                        };

                        await _matchDetailsRepository.AddAsync(matchDetails);

                        matchDetailsCache.Add(matchDetailsData.FixtureId.Value, matchDetails);

                        insertedMatches++;
                    }
                }

                //call save changes anyway
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Match details: updated count: {updatedMatches}, inserted count: {insertedMatches}");

                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;
                _logger.LogInformation($"Job finished... Elapsed time: {elapsed.TotalSeconds} s");

                await _unitOfWork.CommitTransactionAsync(ct);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                _logger.LogError(ex, "An error occurred during job execution");

                throw;
            }
        }
    }
}
