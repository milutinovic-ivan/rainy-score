using Application.Intefraces;
using Application.Models;
using Application.Models.Configuration;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.Intrinsics.Arm;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class MatchOddsImportJob : IJob
    {
        private readonly IMatchLiveService _matchLiveService;
        private readonly ILogger<MatchOddsImportJob> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;
        private readonly MatchLiveProviderSettings _providerSettings;

        public MatchOddsImportJob(IMatchLiveService matchLiveService, 
           ILogger<MatchOddsImportJob> logger,
           IUnitOfWork unitOfWork,
           IRepository<MatchDetails> matchDetailsRepository,
           IConfiguration configuration)
        {
            _matchLiveService = matchLiveService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _matchDetailsRepository = matchDetailsRepository;

            _providerSettings = configuration
                .GetSection($"MatchLiveProviders:{_matchLiveService.ServiceName}")
                .Get<MatchLiveProviderSettings>()
                ?? throw new Exception("Missing match live provider config");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;

            await _unitOfWork.BeginTransactionAsync(ct);

            try
            {
                _logger.LogInformation("Job started");

                int updatedMatchesCount = 0;
                int skippedMatchesCount = 0;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                //get all matches for today, TODO should be date sent to job as parameter
                var nowUtc = DateTime.UtcNow;
                var runDate = DateOnly.FromDateTime(nowUtc);


                //calculate delay, if needed
                TimeSpan? delay = null;
                if (_providerSettings.RequestsPerMinute.HasValue)
                {
                    delay = TimeSpan.FromMinutes(1.0 / _providerSettings.RequestsPerMinute.Value);
                }

                var runDateMatchDetailsList = await _matchDetailsRepository.GetAllAsync(md => md.Where(m => m.MatchDate == runDate));

                //call GetMatchOddsAsync implementation for every match
                foreach (var matchDetails in runDateMatchDetailsList)
                {
                    if(matchDetails.FixtureId is null)
                    {
                        _logger.LogWarning($"Match with id {matchDetails.Id} has no fixture id, match skipped...");
                        continue;
                    }

                    var odds = await _matchLiveService.GetMatchOddsAsync(matchDetails.FixtureId.Value);

                    //apply delay, if needed
                    if(delay.HasValue)
                    {
                        await Task.Delay(delay.Value, ct);
                    }

                    //update match details with odds, should have all odds to update match details
                    if (odds is not null &&
                        odds.HomeWinOdds is not null && 
                        odds.DrawWinOdds is not null && 
                        odds.AwayWinOdds is not null &&
                        odds.GoalsOver25Odds is not null &&
                        odds.GoalsUnder25Odds is not null)
                    {
                        matchDetails.BookmakerId = odds.BookmakerId;
                        matchDetails.BookmakerName = odds.BookmakerName;
                        matchDetails.HomeWinOdds = odds.HomeWinOdds;
                        matchDetails.DrawWinOdds = odds.DrawWinOdds;
                        matchDetails.AwayWinOdds = odds.AwayWinOdds;
                        matchDetails.GoalsOver25Odds = odds.GoalsOver25Odds;
                        matchDetails.GoalsUnder25Odds = odds.GoalsUnder25Odds;
                        matchDetails.OriginalResponseOdds = odds.OriginalResponseOdds;

                        updatedMatchesCount++;
                    }
                    else
                    {
                        //probably this matches should be deleted
                        _logger.LogWarning($"Match with fixture id: {matchDetails.FixtureId.Value} has no odds, match skipped for now...");
                        skippedMatchesCount++;
                    }
                }

                //call save changes anyway
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Match details: updated count: {updatedMatchesCount} ; skipped count: {skippedMatchesCount}");

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
