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

        [HttpPost("run-utc-import-pipeline")]
        public async Task<IActionResult> RunUtcImportPipelineJob()
        {
            return await TriggerJob(new JobKey("UtcImportPipelineJob"));
        }

        [HttpPost("run-match-history-import")]
        public async Task<IActionResult> RunMatchHistoryImportJob()
        {
            return await TriggerJob(new JobKey("MatchHistoryImportJob"));
        }

        [HttpPost("run-stadium-import")]
        public async Task<IActionResult> RunStadiumImportJob()
        {
            return await TriggerJob(new JobKey("StadiumImportJob"));
        }

        [HttpPost("run-weather-history-import")]
        public async Task<IActionResult> RunWeatherHistoryImportJob()
        {
            return await TriggerJob(new JobKey("WeatherHistoryImportJob"));
        }

        [HttpPost("run-match-live-import-today")]
        public async Task<IActionResult> RunMatchLiveImportTodayJob()
        {
            return await TriggerJob(new JobKey("MatchLiveImportJobToday"));
        }

        [HttpPost("run-match-live-import-yesterday")]
        public async Task<IActionResult> RunMatchLiveImportYesterdayJob()
        {
            return await TriggerJob(new JobKey("MatchLiveImportJobYesterday"));
        }

        [HttpPost("run-match-odds-import")]
        public async Task<IActionResult> RunMatchOddsImportJob()
        {
            return await TriggerJob(new JobKey("MatchOddsImportJob"));
        }

        [HttpPost("run-weather-forecast-import")]
        public async Task<IActionResult> RunWeatherForecastImportJob()
        {
            return await TriggerJob(new JobKey("WeatherForecastImportJob"));
        }

        private async Task<IActionResult> TriggerJob(JobKey jobKey)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

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
