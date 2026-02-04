using Application.Intefraces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Diagnostics;

namespace Application.Jobs
{
    public class MatchLiveImportJob : IJob
    {
        private readonly IMatchLiveService _matchLiveService;
        private readonly ILogger<MatchLiveImportJob> _logger;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<League> _leagueRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;
        private readonly IRepository<LeagueExternalMap> _leagueExternalMapRepository;

        public MatchLiveImportJob(IMatchLiveService matchLiveService, ILogger<MatchLiveImportJob> logger,
           IRepository<Country> countryRepository,
           IRepository<League> leagueRepository,
           IRepository<Team> teamRepository,
           IRepository<MatchDetails> matchDetailsRepository,
           IRepository<LeagueExternalMap> leagueExternalMapRepository)
        {
            _matchLiveService = matchLiveService;
            _logger = logger;
            _countryRepository = countryRepository;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _matchDetailsRepository = matchDetailsRepository;
            _leagueExternalMapRepository = leagueExternalMapRepository;
        }

        public async Task Execute(IJobExecutionContext context)
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
            var allleagueExternalMaps = await _leagueExternalMapRepository.GetAllAsync();
            var RunDateMatchDetailsList = await _matchDetailsRepository.GetAllAsync(md => md.Where(m => m.MatchDate == runDate));

            //put results in dicts to not query db every time
            var teamsCache = allTeams.ToDictionary(t => t.Name.Trim(), t => t, StringComparer.OrdinalIgnoreCase);
            var countryCache = allContries.ToDictionary(c => c.Name.Trim(), c => c, StringComparer.OrdinalIgnoreCase);
            var leagueExternalMapCache = allleagueExternalMaps.ToDictionary(l => (l.DataSource, l.ExternalLeagueId), l => l);

            var matchDetailsToInsert = new List<MatchDetails>();
            int updatedMatches = 0;
            int insertedMatches = 0;

            foreach (var matchDetailsData in matchDetailsDataList)
            {
                //match is skipped if there is no league data or competition is cup
                if (matchDetailsData.DataSource is null || matchDetailsData.ExternalLeagueId is null)
                {
                    _logger.LogWarning($"Match skipped... No DataSource or ExternalLeagueId, DataSource: {matchDetailsData.DataSource} ; " +
                        $"ExternalLeagueId: {matchDetailsData.ExternalLeagueId}");
                    continue;
                }

                //if service is implemented get league data from implementation
                LeagueData? leagueData = null;
                if (_matchLiveService is ILeagueService leagueService)
                {
                    leagueData = await leagueService.GetLeagueDataAsync(matchDetailsData.ExternalLeagueId.Value);

                    if (leagueData is null)
                    {
                        _logger.LogWarning($"Match skipped... LeagueService returned null");
                        continue;
                    }

                    if(leagueData.IsCup)
                    {
                        _logger.LogInformation($"Match skipped... it is cup not league");
                        continue;
                    }
                }
                else
                {
                    bool isCup = matchDetailsData.IsCup ?? false;
                    if(isCup)
                    {
                        _logger.LogInformation($"Match skipped... it is cup not league");
                        continue;
                    }
                }

                //add home team just if already not exists
                if (!teamsCache.TryGetValue(matchDetailsData.HomeTeam, out var homeTeam))
                {
                    homeTeam = new Team
                    {
                        Name = matchDetailsData.HomeTeam,
                        StadiumId = null
                    };

                    await _teamRepository.AddAsync(homeTeam);
                    await _teamRepository.SaveChangesAsync();

                    //add new item in dict
                    teamsCache.Add(homeTeam.Name, homeTeam);

                    _logger.LogInformation($"Team added... Name: {homeTeam.Name}");
                }

                //add away team just if already not exists
                if (!teamsCache.TryGetValue(matchDetailsData.AwayTeam, out var awayTeam))
                {
                    awayTeam = new Team
                    {
                        Name = matchDetailsData.AwayTeam,
                        StadiumId = null
                    };

                    await _teamRepository.AddAsync(awayTeam);
                    await _teamRepository.SaveChangesAsync();

                    //add new item in dict
                    teamsCache.Add(awayTeam.Name, awayTeam);

                    _logger.LogInformation($"Team added... Name: {awayTeam.Name}");
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
                    await _countryRepository.SaveChangesAsync();

                    countryCache.Add(normalizedCountryName, country);

                    _logger.LogInformation($"Country added... Name: {country.Name}");
                }


                //leagues
                var leagueExternalMapKey = (matchDetailsData.DataSource, matchDetailsData.ExternalLeagueId.Value);

                if (!leagueExternalMapCache.TryGetValue(leagueExternalMapKey, out var map))
                {
                    //insert new League
                    var league = new League();

                    //if service is implemented get league data from implementation
                    if(_matchLiveService is ILeagueService)
                    {
                        league.Name = leagueData!.Name;
                        league.CountryId = country.Id;
                        league.IsCup = leagueData.IsCup;
                    }
                    else
                    {
                        league.Name = matchDetailsData.LeagueName!;
                        league.CountryId = country.Id;
                        league.IsCup = matchDetailsData.IsCup ?? false;
                    }

                    await _leagueRepository.AddAsync(league);
                    await _leagueRepository.SaveChangesAsync();

                    _logger.LogInformation($"League added... Name: {league.Name}");

                    //insert new LeagueExternalMap
                    map = new LeagueExternalMap
                    {
                        LeagueId = league.Id,
                        DataSource = matchDetailsData.DataSource,
                        ExternalLeagueId = matchDetailsData.ExternalLeagueId.Value
                    };

                    await _leagueExternalMapRepository.AddAsync(map);
                    await _leagueExternalMapRepository.SaveChangesAsync();

                    //add new LeagueExternalMap in cache dict
                    leagueExternalMapCache.Add(leagueExternalMapKey, map);
                }

                var leagueId = map.LeagueId;

                //MatchDetails
                var existingMatch = RunDateMatchDetailsList.SingleOrDefault(md =>
                    md.LeagueId == leagueId &&
                    md.HomeTeamId == homeTeam.Id &&
                    md.AwayTeamId == awayTeam.Id &&
                    md.Season == matchDetailsData.MatchDate.Year &&
                    md.MatchDate == matchDetailsData.MatchDate);

                // update or insert new MatchDetails
                if (existingMatch != null)
                {
                    existingMatch.FullTimeHomeGoals = matchDetailsData.FullTimeHomeGoals;
                    existingMatch.FullTimeAwayGoals = matchDetailsData.FullTimeAwayGoals;
                    existingMatch.FullTimeWiner = matchDetailsData.FullTimeWiner;
                    existingMatch.HalfTimeHomeGoals = matchDetailsData.HalfTimeHomeGoals;
                    existingMatch.HalfTimeAwayGoals = matchDetailsData.HalfTimeAwayGoals;
                    existingMatch.HalfTimeWiner = matchDetailsData.HalfTimeWiner;

                    updatedMatches++;
                }
                else
                {
                    var matchDetails = new MatchDetails
                    {
                        LeagueId = leagueId,
                        Season = matchDetailsData.MatchDate.Year,
                        MatchDate = matchDetailsData.MatchDate,
                        MatchTime = matchDetailsData.MatchTime,
                        HomeTeamId = homeTeam.Id,
                        AwayTeamId = awayTeam.Id,
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

                    matchDetailsToInsert.Add(matchDetails);

                    insertedMatches++;
                }
            }

            //insert all at once
            if(matchDetailsToInsert.Count > 0)
            {
                await _matchDetailsRepository.AddRangeAsync(matchDetailsToInsert);
            }

            //call save changes anyway
            await _matchDetailsRepository.SaveChangesAsync();

            _logger.LogInformation($"Match details: updated count: {updatedMatches}, inserted count: {insertedMatches}");

            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            _logger.LogInformation($"Job finished... Elapsed time: {elapsed.TotalSeconds} s");
        }
    }
}
