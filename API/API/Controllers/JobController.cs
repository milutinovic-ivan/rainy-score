using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace API.Controllers
{
    [Route("api/job")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<JobController> _logger;

        public JobController(ISchedulerFactory schedulerFactory, ILogger<JobController> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        [HttpPost("run-match-history-import")]
        public async Task<IActionResult> RunMatchHistoryImportJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobKey = new JobKey("MatchHistoryImportJob");

            if (!await scheduler.CheckExists(jobKey))
            {
                return NotFound("Job not registered in scheduler.");
            }

            _logger.LogInformation("Manually triggering job at {Time}", DateTime.Now);

            //only queue job for execution, not wait untill job finish
            await scheduler.TriggerJob(jobKey);

            return Ok("Job triggered successfully.");
        }

        [HttpPost("run-stadium-import")]
        public async Task<IActionResult> RunStadiumImportJob()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobKey = new JobKey("StadiumImportJob");

            if (!await scheduler.CheckExists(jobKey))
            {
                return NotFound("Job not registered in scheduler.");
            }

            _logger.LogInformation("Manually triggering job at {Time}", DateTime.Now);

            //only queue job for execution, not wait untill job finish
            await scheduler.TriggerJob(jobKey);

            return Ok("Job triggered successfully.");
        }
    }
}
