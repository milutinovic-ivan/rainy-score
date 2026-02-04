using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;

namespace Application.Jobs
{
    public class MatchLiveImportJob : IJob
    {
        private readonly IMatchLiveService _matchLiveService;
        private readonly ILogger<MatchLiveImportJob> _logger;
        private readonly IRepository<League> _leagueRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;

        public MatchLiveImportJob(IMatchLiveService matchLiveService, ILogger<MatchLiveImportJob> logger,
           IRepository<League> leagueRepository,
           IRepository<Team> teamRepository,
           IRepository<MatchDetails> matchDetailsRepository)
        {
            _matchLiveService = matchLiveService;
            _logger = logger;
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _matchDetailsRepository = matchDetailsRepository;
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
            if(teamsToInsert.Count > 0)
            {
                await _teamRepository.AddRangeAsync(teamsToInsert);
                await _teamRepository.SaveChangesAsync();
            }

            _logger.LogInformation($"Teams inserted count: {teamsToInsert.Count}");


            //insert MatchDetails
            var existingMatchDetailsList = await _matchDetailsRepository.GetAllAsync(md => md.Where(m => m.MatchDate == runDate));
            var allLeagues = await _leagueRepository.GetAllAsync();
            var allTeams = await _teamRepository.GetAllAsync();

            var matchDetailsToInsert = new List<MatchDetails>();

            foreach (var matchDetailsData in matchDetailsDataList) 
            {
                var league = allLeagues.Single(l => l.ShortCode == matchDetailsData.LeagueDivision);
                var homeTeam = allTeams.Single(t => t.Name == matchDetailsData.HomeTeam);
                var awayTeam = allTeams.Single(t => t.Name == matchDetailsData.AwayTeam);

                //TODO
                // 2. if league doesn't exists, get league from ILeagueService interface and insert into database
                // 3. also check if country exists and insert Country first if not
                // 4. just if league is not IsCup then continue with match insert
                // 5. add other fields in MatchDetailData like Status, OriginalReponse... 
                // 6. create new service method and return odds for single match, insert or update odds also
                // 7. take care when returning odds to not break 1 minute limit, and maybe day limit also

                var existingMatch = existingMatchDetailsList.SingleOrDefault(md =>
                    md.LeagueId == league.Id &&
                    md.HomeTeamId == homeTeam.Id &&
                    md.AwayTeamId == awayTeam.Id &&
                    md.Season == matchDetailsData.MatchDate.Year &&
                    md.MatchDate == matchDetailsData.MatchDate);

                //if match exists update, if not insert new
                if(existingMatch != null)
                {
                    existingMatch.FullTimeHomeGoals = matchDetailsData.FullTimeHomeGoals;
                    existingMatch.FullTimeAwayGoals = matchDetailsData.FullTimeAwayGoals;
                    existingMatch.FullTimeWiner = matchDetailsData.FullTimeWiner;
                    existingMatch.HalfTimeHomeGoals = matchDetailsData.HalfTimeHomeGoals;
                    existingMatch.HalfTimeAwayGoals = matchDetailsData.HalfTimeAwayGoals;
                    existingMatch.HalfTimeWiner = matchDetailsData.HalfTimeWiner;
                    existingMatch.HomeWinOdds = matchDetailsData.HomeWinOdds;
                    existingMatch.DrawWinOdds = matchDetailsData.DrawWinOdds;
                    existingMatch.AwayWinOdds = matchDetailsData.AwayWinOdds;
                    existingMatch.GoalsOver25Odds = matchDetailsData.GoalsOver25Odds;
                    existingMatch.GoalsUnder25Odds = matchDetailsData.GoalsUnder25Odds;
                    existingMatch.IsHistory = matchDetailsData.IsHistory;
                }
                else
                {
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
                        HalfTimeAwayGoals = matchDetailsData.HalfTimeAwayGoals,
                        HalfTimeWiner = matchDetailsData.HalfTimeWiner,
                        HomeWinOdds = matchDetailsData.HomeWinOdds,
                        DrawWinOdds = matchDetailsData.DrawWinOdds,
                        AwayWinOdds = matchDetailsData.AwayWinOdds,
                        GoalsOver25Odds = matchDetailsData.GoalsOver25Odds,
                        GoalsUnder25Odds = matchDetailsData.GoalsUnder25Odds,
                        IsHistory = matchDetailsData.IsHistory
                    };

                    matchDetailsToInsert.Add(matchDetails);
                }
            }

            //insert all at once
            if(matchDetailsToInsert.Count > 0)
            {
                await _matchDetailsRepository.AddRangeAsync(matchDetailsToInsert);
            }

            //call save changes anyway
            await _matchDetailsRepository.SaveChangesAsync();

            _logger.LogInformation($"Match details inserted count: {matchDetailsToInsert.Count}");

            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            _logger.LogInformation($"Job finished... Elapsed time: {elapsed.TotalSeconds} s");
        }
    }
}
