using Quartz;
using Application.Intefraces;
using Microsoft.Extensions.Logging;

namespace Application.Jobs
{
    public class MatchHistoryImportJob : IJob
    {
        private readonly IMatchHistoryService _matchHistoryService;
        private readonly ILogger<MatchHistoryImportJob> _logger;

        public MatchHistoryImportJob(IMatchHistoryService matchHistoryService, ILogger<MatchHistoryImportJob> logger)
        {
            _matchHistoryService = matchHistoryService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var matchHistories = await _matchHistoryService.GetMatchHistoriesAsync();

            foreach(var matchHistory in matchHistories)
            {

            }
        }
    }
}
