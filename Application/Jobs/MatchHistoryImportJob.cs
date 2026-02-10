using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
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

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //call service implementation to get list of match details
            var matchDetailsDataList = await _matchHistoryService.GetMatchDetailsHistoriesAsync();

            //insert Teams
            var existingTeams = await _teamRepository.GetAllAsync();
            var teamsToInsert = new List<Team>();

            foreach (var matchDetailsData in matchDetailsDataList)
            {
                //add home team just if already not exists
                var teamExists = existingTeams.Any(t => t.Name == matchDetailsData.HomeTeam);

                if (!teamExists)
                {
                    var team = new Team
                    {
                        Name = matchDetailsData.HomeTeam,
                        StadiumId = null
                    };

                    teamsToInsert.Add(team);
                }

                //add away team just if already not exists
                teamExists = existingTeams.Any(t => t.Name == matchDetailsData.AwayTeam);

                if (!teamExists)
                {
                    var team = new Team
                    {
                        Name = matchDetailsData.AwayTeam,
                        StadiumId = null
                    };

                    teamsToInsert.Add(team);
                }
            }

            //save all at once
            if (teamsToInsert.Count > 0)
            {
                await _teamRepository.AddRangeAsync(teamsToInsert);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation($"Teams inserted count: {teamsToInsert.Count}");


            //insert MatchDetails
            var existingMatchDetails = await _matchDetailsRepository.GetAllAsync();
            var allLeagues = await _leagueRepository.GetAllAsync();
            var allTeams = await _teamRepository.GetAllAsync();

            var matchDetailsToInsert = new List<MatchDetails>();

            int matchSkipped = 0;

            foreach (var matchDetailsData in matchDetailsDataList)
            {
                var league = allLeagues.Single(l => l.ShortCode == matchDetailsData.LeagueDivision);
                var homeTeam = allTeams.Single(t => t.Name == matchDetailsData.HomeTeam);
                var awayTeam = allTeams.Single(t => t.Name == matchDetailsData.AwayTeam);

                //match already exists in database, skip match
                if (existingMatchDetails.Any(md =>
                md.LeagueId == league.Id &&
                md.HomeTeamId == homeTeam.Id &&
                md.AwayTeamId == awayTeam.Id &&
                md.Season == matchDetailsData.MatchDate.Year &&
                md.MatchDate == matchDetailsData.MatchDate))
                {
                    matchSkipped++;
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
                    HomeTeamId = homeTeam.Id,
                    AwayTeamId = awayTeam.Id,
                    FullTimeHomeGoals = matchDetailsData.FullTimeHomeGoals,
                    FullTimeAwayGoals = matchDetailsData.FullTimeAwayGoals,
                    FullTimeWiner = matchDetailsData.FullTimeWiner,
                    HalfTimeHomeGoals = matchDetailsData.HalfTimeHomeGoals,
                    HalfTimeAwayGoals= matchDetailsData.HalfTimeAwayGoals,
                    HalfTimeWiner= matchDetailsData.HalfTimeWiner,
                    HomeWinOdds = matchDetailsData.HomeWinOdds,
                    DrawWinOdds = matchDetailsData.DrawWinOdds,
                    AwayWinOdds = matchDetailsData.AwayWinOdds,
                    GoalsOver25Odds = matchDetailsData.GoalsOver25Odds,
                    GoalsUnder25Odds = matchDetailsData.GoalsUnder25Odds,
                    IsHistory = matchDetailsData.IsHistory
                };

                matchDetailsToInsert.Add(matchDetails);
            }

            //insert all at once
            if(matchDetailsToInsert.Count > 0)
            {
                await _matchDetailsRepository.AddRangeAsync(matchDetailsToInsert);
                await _unitOfWork.SaveChangesAsync();
            }

            _logger.LogInformation($"Match details inserted count: {matchDetailsToInsert.Count}, skipped: {matchSkipped}");

            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            _logger.LogInformation($"Job finished... Elapsed time: {elapsed.TotalSeconds} s");
        }
    }
}
