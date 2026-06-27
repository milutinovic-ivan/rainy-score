using Application.Intefraces;
using Application.Jobs.Services;
using Application.Models;
using Application.Models.Configuration;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;

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
        private readonly IJobExecutionsService _jobExecutionsService;

        public MatchLiveImportJob(IMatchLiveService matchLiveService, ILogger<MatchLiveImportJob> logger,
           IRepository<Country> countryRepository,
           IRepository<League> leagueRepository,
           IRepository<Team> teamRepository,
           IRepository<MatchDetails> matchDetailsRepository,
           IRepository<LeagueExternalMap> leagueExternalMapRepository,
           IUnitOfWork unitOfWork, 
           IConfiguration configuration,
           IJobExecutionsService jobExecutionsService)
        {
            _matchLiveService = matchLiveService;
            _logger = logger;
            _countryRepository = countryRepository;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _matchDetailsRepository = matchDetailsRepository;
            _leagueExternalMapRepository = leagueExternalMapRepository;
            _unitOfWork = unitOfWork;
            _jobExecutionsService = jobExecutionsService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;

            var executionId = await _jobExecutionsService.StartAsync(nameof(MatchLiveImportJob));

            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                _logger.LogInformation("Job started");

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14));

                //job could be run for today or yesterday
                var dateOffsetDays = context.MergedJobDataMap.GetIntValue("DateOffsetDays");
                var runDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dateOffsetDays));
                _logger.LogInformation($"Importing live matches for date: {runDate}, date offset days: {dateOffsetDays}");

                //call service implementation to get list of match details
                var matchDetailsDataList = await _matchLiveService.GetMatchDetailsListAsync(runDate);
                var serviceName = _matchLiveService.GetServiceName;

                //get all existing data from db
                var allTeams = await _teamRepository.GetAllAsync(t => t.Include(t => t.Country));
                var allContries = await _countryRepository.GetAllAsync();
                var allLeagues = await _leagueRepository.GetAllAsync(l => l.Include(l => l.Country));
                var allleagueExternalMaps = await _leagueExternalMapRepository.GetAllAsync(lem => lem.Include(lem => lem.League));
                var lastDaysMatchDetailsList = await _matchDetailsRepository.GetAllAsync(md => md.Where(m => m.DataSource == serviceName && m.MatchDate >= fromDate));

                //put results in dicts to not query db every time
                var teamsCache = allTeams.ToDictionary(t => (t.Name.Trim(), t.Country!.Name.Trim()), t => t);
                var countryCache = allContries.ToDictionary(c => c.Name.Trim(), c => c, StringComparer.OrdinalIgnoreCase);
                var leagueCache = allLeagues.ToDictionary(l => (l.Name.Trim(), l.Country.Name.Trim()), l => l);
                var leagueExternalMapCache = allleagueExternalMaps.ToDictionary(l => (l.DataSource, l.ExternalLeagueId), l => l);
                var matchDetailsCache = lastDaysMatchDetailsList.Where(md => md.FixtureId != null).ToDictionary(md => md.FixtureId!.Value, md => md);

                int updatedMatches = 0;
                int insertedMatches = 0;
                int skippedMatchesWithoutOdds = 0;
                int skippedMatchesWithoutFullTimeResult = 0;
                int skippedMatchesIsCup = 0;
                int teamsAdded = 0;

                foreach (var matchDetailsData in matchDetailsDataList)
                {
                    //match is skipped if there is no fixture id or data source or external league id
                    if (matchDetailsData.FixtureId is null || matchDetailsData.DataSource is null || matchDetailsData.ExternalLeagueId is null)
                    {
                        _logger.LogWarning($"Match skipped... No FixtureId or DataSource or ExternalLeagueId, " +
                            $"FixtureId: {matchDetailsData.FixtureId} ; " +
                            $"DataSource: {matchDetailsData.DataSource} ; " +
                            $"ExternalLeagueId: {matchDetailsData.ExternalLeagueId}");

                        continue;
                    }

                    //get league info, if it is cup skip match
                    LeagueData? leagueData = await GetLeagueInfoAsync(matchDetailsData);
                    if(leagueData is null || leagueData.IsCup)
                    {
                        skippedMatchesIsCup++;
                        _logger.LogWarning($"Match skipped... league is null or it is cup not league, LeagueName: {leagueData?.Name} ; ExternalLeagueId: {matchDetailsData.ExternalLeagueId}");

                        continue;
                    }

                    //if match is finished but there is no full time result, remove match if exists and skip
                    if (matchDetailsData.Status == "FT" && (matchDetailsData.FullTimeHomeGoals is null || matchDetailsData.FullTimeAwayGoals is null))
                    {
                        if(matchDetailsCache.TryGetValue(matchDetailsData.FixtureId.Value, out var matchToDelete))
                        {
                            _matchDetailsRepository.Delete(matchToDelete);
                            _logger.LogInformation($"Match deleted... match is finished but there is no full time result");
                            skippedMatchesWithoutFullTimeResult++;

                            continue;
                        }

                        skippedMatchesWithoutFullTimeResult++;
                        _logger.LogInformation($"Match skipped... match is finished but there is no full time result");

                        continue;
                    }

                    //get match odds
                    var odds = await _matchLiveService.GetMatchOddsAsync(matchDetailsData.FixtureId.Value);

                    if (!HasRequiredOdds(odds))
                    {
                        _logger.LogWarning($"Match skipped... No odds data, FixtureId: {matchDetailsData.FixtureId.Value}");
                        skippedMatchesWithoutOdds++;

                        continue;
                    }

                    //set ods data
                    matchDetailsData.BookmakerId = odds.BookmakerId;
                    matchDetailsData.BookmakerName = odds.BookmakerName;
                    matchDetailsData.HomeWinOdds = odds.HomeWinOdds;
                    matchDetailsData.DrawWinOdds = odds.DrawWinOdds;
                    matchDetailsData.AwayWinOdds = odds.AwayWinOdds;
                    matchDetailsData.GoalsOver25Odds = odds.GoalsOver25Odds;
                    matchDetailsData.GoalsUnder25Odds = odds.GoalsUnder25Odds;
                    matchDetailsData.OriginalResponseOdds = odds.OriginalResponseOdds;

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


                    //leagues, one league can have multiple external maps, this means multiple DataSources(ApiFootball, OddsApi...)
                    //first check if external map exists, if not add it but use existing league, if league exists with that name for that country or add new league
                    var leagueExternalMapKey = (matchDetailsData.DataSource, matchDetailsData.ExternalLeagueId.Value);

                    var leagueKey = (matchDetailsData.LeagueName!.Trim(), country.Name.Trim());

                    if (!leagueExternalMapCache.TryGetValue(leagueExternalMapKey, out var leagueExternalMap))
                    {
                        //add new league just if not exists
                        if (!leagueCache.TryGetValue(leagueKey, out var league))
                        {
                            //insert new League
                            league = new League();

                            league.Name = leagueData.Name;
                            league.Country = country;
                            league.IsCup = leagueData.IsCup;

                            //add new league to database and dictonary
                            await _leagueRepository.AddAsync(league);
                            leagueCache.Add(leagueKey, league);

                            _logger.LogInformation($"League added... Name: {league.Name}");
                        }

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


                    //add home team just if already not exists
                    var homeTeamKey = (matchDetailsData.HomeTeam.Trim(), country.Name);
                    if (!teamsCache.TryGetValue(homeTeamKey, out var homeTeam))
                    {
                        homeTeam = new Team
                        {
                            Name = matchDetailsData.HomeTeam.Trim(),
                            Country = country,
                            StadiumId = null
                        };

                        await _teamRepository.AddAsync(homeTeam);

                        //add new item in dict
                        teamsCache.Add(homeTeamKey, homeTeam);

                        _logger.LogDebug($"Team added... Name: {homeTeam.Name}");
                        teamsAdded++;
                    }

                    //add away team just if already not exists
                    var awayTeamKey = (matchDetailsData.AwayTeam.Trim(), country.Name);
                    if (!teamsCache.TryGetValue(awayTeamKey, out var awayTeam))
                    {
                        awayTeam = new Team
                        {
                            Name = matchDetailsData.AwayTeam.Trim(),
                            Country = country,
                            StadiumId = null
                        };

                        await _teamRepository.AddAsync(awayTeam);

                        //add new item in dict
                        teamsCache.Add(awayTeamKey, awayTeam);

                        _logger.LogDebug($"Team added... Name: {awayTeam.Name}");
                        teamsAdded++;
                    }

                    //MatchDetails

                    if(matchDetailsCache.TryGetValue(matchDetailsData.FixtureId.Value, out var existingMatch))
                    {
                        existingMatch.MatchDate = matchDetailsData.MatchDate;
                        existingMatch.MatchTime = matchDetailsData.MatchTime;
                        existingMatch.FullTimeHomeGoals = matchDetailsData.FullTimeHomeGoals;
                        existingMatch.FullTimeAwayGoals = matchDetailsData.FullTimeAwayGoals;
                        existingMatch.FullTimeWiner = matchDetailsData.FullTimeWiner;
                        existingMatch.HalfTimeHomeGoals = matchDetailsData.HalfTimeHomeGoals;
                        existingMatch.HalfTimeAwayGoals = matchDetailsData.HalfTimeAwayGoals;
                        existingMatch.HalfTimeWiner = matchDetailsData.HalfTimeWiner;
                        existingMatch.Status = matchDetailsData.Status;
                        existingMatch.BookmakerId = matchDetailsData.BookmakerId;
                        existingMatch.BookmakerName = matchDetailsData.BookmakerName;
                        existingMatch.HomeWinOdds = matchDetailsData.HomeWinOdds;
                        existingMatch.DrawWinOdds = matchDetailsData.DrawWinOdds;
                        existingMatch.AwayWinOdds = matchDetailsData.AwayWinOdds;
                        existingMatch.GoalsOver25Odds = matchDetailsData.GoalsOver25Odds;
                        existingMatch.GoalsUnder25Odds = matchDetailsData.GoalsUnder25Odds;
                        existingMatch.OriginalResponseOdds = matchDetailsData.OriginalResponseOdds;

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
                            Status = matchDetailsData.Status,
                            BookmakerId = matchDetailsData.BookmakerId,
                            BookmakerName = matchDetailsData.BookmakerName,
                            OriginalResponseOdds = matchDetailsData.OriginalResponseOdds
                        };

                        await _matchDetailsRepository.AddAsync(matchDetails);

                        matchDetailsCache.Add(matchDetailsData.FixtureId.Value, matchDetails);

                        insertedMatches++;
                    }
                }

                //call save changes anyway
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Teams added count: {teamsAdded}");
                _logger.LogInformation($"Match details: updated count: {updatedMatches}, inserted count: {insertedMatches}");
                _logger.LogInformation($"Match details: skipped without odds count: {skippedMatchesWithoutOdds}");
                _logger.LogInformation($"Match details: skipped without full time result count: {skippedMatchesWithoutFullTimeResult}");
                _logger.LogInformation($"Match details: skipped because it is cup count: {skippedMatchesIsCup}");

                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;
                _logger.LogInformation($"Job finished... Elapsed time: {elapsed.TotalSeconds} s");

                await _unitOfWork.CommitTransactionAsync(ct);

                //log finished job metrics
                var metricsJson = new
                {
                    teamsAdded,
                    updatedMatches,
                    insertedMatches,
                    skippedMatchesWithoutOdds,
                    skippedMatchesWithoutFullTimeResult,
                    skippedMatchesIsCup
                };

                await _jobExecutionsService.FinishAsync(executionId, JobExecutionStatus.Success, metricsJson);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                _logger.LogError(ex, "An error occurred during job execution");

                await _jobExecutionsService.FinishAsync(executionId, JobExecutionStatus.Failed, null, ex);

                throw;
            }
        }

        private bool HasRequiredOdds(MatchDetailsData? odds)
        {
            return odds is not null
                && odds.HomeWinOdds is not null
                && odds.DrawWinOdds is not null
                && odds.AwayWinOdds is not null
                && odds.GoalsOver25Odds is not null
                && odds.GoalsUnder25Odds is not null;
        }

        private async Task<LeagueData?> GetLeagueInfoAsync(MatchDetailsData matchDetailsData)
        {
            LeagueData? leagueData;

            //match is skipped if there is no league data or competition is cup
            if (_matchLiveService is ILeagueService leagueService)
            {
                leagueData = await leagueService.GetLeagueDataAsync(matchDetailsData.ExternalLeagueId!.Value);
            }
            else
            {
                leagueData = new LeagueData();

                leagueData.Name = matchDetailsData.LeagueName!;
                leagueData.IsCup = matchDetailsData.IsCup ?? false;
            }

            return leagueData;
        }
    }
}
