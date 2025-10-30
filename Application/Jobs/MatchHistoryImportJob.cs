using Quartz;
using Application.Intefraces;
using Microsoft.Extensions.Logging;
using Domain.Interfaces;
using Domain.Entities;
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
            var matches = await _matchHistoryService.GetMatchHistoriesAsync();

            foreach(var match in matches)
            {
                var league = await _leagueRepository.SingleOrDefaultAsync(l => l.Where(l => l.ShortCode == match.LeagueDivision));

                Team? homeTeam = await _teamRepository.SingleOrDefaultAsync(t => t.Where(t => t.Name == match.HomeTeam));
                Team? awayTeam = await _teamRepository.SingleOrDefaultAsync(t => t.Where(t => t.Name == match.AwayTeam));

                //add teams if doesn't exists
                if (homeTeam == null)
                {
                    homeTeam = new Team
                    {
                        Name = match.HomeTeam,
                        //TODO stadiums should be added
                        StadiumId = 1
                    };

                    await _teamRepository.AddAsync(homeTeam);
                    await _teamRepository.SaveChangesAsync();
                }

                if (awayTeam == null)
                {
                    awayTeam = new Team
                    {
                        Name = match.AwayTeam,
                        StadiumId = 1
                    };

                    await _teamRepository.AddAsync(awayTeam);
                    await _teamRepository.SaveChangesAsync();
                }

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

                await _matchDetailsRepository.AddAsync(matchDetails);
            }

            await _matchDetailsRepository.SaveChangesAsync();
        }
    }
}
