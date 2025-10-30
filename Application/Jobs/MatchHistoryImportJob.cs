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

        public MatchHistoryImportJob(IMatchHistoryService matchHistoryService, ILogger<MatchHistoryImportJob> logger,
           IRepository<League> leagueRepository,
           IRepository<Team> teamRepository,
           IRepository<MatchDetails> matchDetailsRepository)
        {
            _matchHistoryService = matchHistoryService;
            _logger = logger;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _matchDetailsRepository = matchDetailsRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //first delete all data from tables Teams and MatchDetails
            await _teamRepository.DeleteAll();
            await _matchDetailsRepository.DeleteAll();
            
            var matches = await _matchHistoryService.GetMatchHistoriesAsync();

            var teamsToInsert = new List<Team>();

            //insert Teams
            foreach (var match in matches)
            {
                var teamExists = teamsToInsert.Exists(t => t.Name == match.HomeTeam);

                //add teams if doesn't exists
                if (!teamExists)
                {
                    var team = new Team
                    {
                        Name = match.HomeTeam,
                        //TODO stadiums should be added
                        StadiumId = 1
                    };

                    teamsToInsert.Add(team);
                }

                teamExists = teamsToInsert.Exists(t => t.Name == match.AwayTeam);

                if (!teamExists)
                {
                    var team = new Team
                    {
                        Name = match.AwayTeam,
                        StadiumId = 1
                    };

                    teamsToInsert.Add(team);
                }
            }
            //save all at once
            await _teamRepository.AddRangeAsync(teamsToInsert);
            await _teamRepository.SaveChangesAsync();


            //insert MatchDetails
            var matchDetailsToInsert = new List<MatchDetails>();
            var allLeagues = await _leagueRepository.GetAllAsync();
            var allTeams = await _teamRepository.GetAllAsync();

            foreach (var match in matches) 
            {
                var league = allLeagues.Single(l => l.ShortCode == match.LeagueDivision);
                var homeTeam = allTeams.Single(t => t.Name == match.HomeTeam);
                var awayTeam = allTeams.Single(t => t.Name == match.AwayTeam);

                var matchDetails = new MatchDetails
                {
                    LeagueId = league.Id,
                    Season = match.MatchDate.Year,
                    MatchDate = match.MatchDate,
                    MatchTime = match.MatchTime,
                    HomeTeamId = homeTeam.Id,
                    AwayTeamId = awayTeam.Id,
                    FullTimeHomeGoals = match.FullTimeHomeGoals,
                    FullTimeAwayGoals = match.FullTimeAwayGoals,
                    FullTimeWiner = match.FullTimeWiner,
                    HalfTimeHomeGoals = match.HalfTimeHomeGoals,
                    HalfTimeAwayGoals= match.HalfTimeAwayGoals,
                    HalfTimeWiner= match.HalfTimeWiner,
                    HomeShots = match.HomeShots,
                    AwayShots = match.AwayShots,
                    HomeShotsOnTarget = match.HomeShotsOnTarget,
                    AwayShotsOnTarget = match.AwayShotsOnTarget,
                    HomeWinOdds = match.HomeWinOdds,
                    DrawWinOdds = match.DrawWinOdds,
                    AwayWinOdds = match.AwayWinOdds,
                    GoalsOver25 = match.GoalsOver25,
                    GoalsUnder25 = match.GoalsUnder25
                };

                matchDetailsToInsert.Add(matchDetails);
            }

            //insert all at once
            await _matchDetailsRepository.AddRangeAsync(matchDetailsToInsert);
            await _matchDetailsRepository.SaveChangesAsync();

            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} ms");
        }
    }
}
