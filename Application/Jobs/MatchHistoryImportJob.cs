using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Application.Jobs
{
    public class MatchHistoryImportJob : IJob
    {
        private readonly IMatchHistoryService _matchHistoryService;
        private readonly ILogger<MatchHistoryImportJob> _logger;
        private readonly IRepository<League> _leagueRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MatchHistoryImportJob(IMatchHistoryService matchHistoryService, ILogger<MatchHistoryImportJob> logger,
           IRepository<League> leagueRepository,
           IRepository<Team> teamRepository,
           IRepository<MatchDetails> matchDetailsRepository,
           IUnitOfWork unitOfWork)
        {
            _matchHistoryService = matchHistoryService;
            _logger = logger;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _matchDetailsRepository = matchDetailsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Job started");

            int teamAddedCount = 0;
            int matchAddedCount = 0;
            int matchSkippedCount = 0;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //call service implementation to get list of match details
            var matchDetailsDataList = await _matchHistoryService.GetMatchDetailsHistoriesAsync();

            //insert Teams
            var allLeagues = await _leagueRepository.GetAllAsync(l => l.Include(l => l.Country));
            var allTeams = await _teamRepository.GetAllAsync(t => t.Include(t => t.Country));

            var allMatchDetails = await _matchDetailsRepository.GetAllAsync(md => 
                md.Include(md => md.HomeTeam)
                .Include(md => md.AwayTeam));

            //put teams in dict
            var teamsCache = allTeams.ToDictionary(t => (t.Name.Trim(), t.Country!.Name.Trim()), t => t);

            foreach (var matchDetailsData in matchDetailsDataList)
            {
                var league = allLeagues.SingleOrDefault(l => l.ShortCode == matchDetailsData.LeagueDivision);
                if (league == null)
                {
                    _logger.LogError($"Match skipped: no existing league with ShortCode: {matchDetailsData.LeagueDivision}");
                    continue;
                }

                //add home team just if already not exists
                var homeTeamKey = (matchDetailsData.HomeTeam.Trim(), league.Country.Name.Trim());
                if (!teamsCache.TryGetValue(homeTeamKey, out var homeTeam))
                {
                    homeTeam = new Team
                    {
                        Name = matchDetailsData.HomeTeam.Trim(),
                        CountryId = league.CountryId,
                        StadiumId = null
                    };

                    await _teamRepository.AddAsync(homeTeam);

                    //add new item in dict
                    teamsCache.Add(homeTeamKey, homeTeam);

                    _logger.LogDebug($"Team added... Name: {homeTeam.Name}");

                    teamAddedCount++;
                }

                //add away team just if already not exists
                var awayTeamKey = (matchDetailsData.AwayTeam.Trim(), league.Country.Name.Trim());
                if (!teamsCache.TryGetValue(awayTeamKey, out var awayTeam))
                {
                    awayTeam = new Team
                    {
                        Name = matchDetailsData.AwayTeam.Trim(),
                        CountryId = league.CountryId,
                        StadiumId = null
                    };

                    await _teamRepository.AddAsync(awayTeam);

                    //add new item in dict
                    teamsCache.Add(awayTeamKey, awayTeam);

                    _logger.LogDebug($"Team added... Name: {awayTeam.Name}");
                    teamAddedCount++;
                }

                //match already exists in database, skip match
                if (allMatchDetails.Any(md =>
                md.LeagueId == league.Id &&
                md.HomeTeam.Name == homeTeam.Name &&
                md.AwayTeam.Name == awayTeam.Name &&
                md.Season == matchDetailsData.MatchDate.Year &&
                md.MatchDate == matchDetailsData.MatchDate))
                {
                    matchSkippedCount++;
                    continue;
                }

                //match doensn't exist in database but data is not complete, no odds, skip match
                if (matchDetailsData.HomeWinOdds == null
                    || matchDetailsData.DrawWinOdds == null
                    || matchDetailsData.AwayWinOdds == null
                    || matchDetailsData.GoalsOver25Odds == null
                    || matchDetailsData.GoalsUnder25Odds == null)
                {
                    continue;
                }

                var matchDetails = new MatchDetails
                {
                    LeagueId = league.Id,
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
                    IsHistory = matchDetailsData.IsHistory
                };

                await _matchDetailsRepository.AddAsync(matchDetails);
                matchAddedCount++;
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Team added count: {teamAddedCount} Match added count: {matchAddedCount}, match skipped: {matchSkippedCount}");
        }
    }
}
