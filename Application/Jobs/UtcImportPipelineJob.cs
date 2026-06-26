using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class UtcImportPipelineJob : IJob
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<UtcImportPipelineJob> _logger;

        public UtcImportPipelineJob(
            ISchedulerFactory schedulerFactory,
            ILogger<UtcImportPipelineJob> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);
            var weatherForecastHourlyTriggerKey = new TriggerKey("WeatherForecastImportJobHourlyTrigger");

            _logger.LogInformation("UTC import pipeline started");

            await scheduler.PauseTrigger(weatherForecastHourlyTriggerKey, context.CancellationToken);

            try
            {
                await TriggerAndWaitAsync(
                    scheduler,
                    new JobKey("MatchLiveImportJobToday"),
                    null,
                    context.CancellationToken);

                await TriggerAndWaitAsync(
                    scheduler,
                    new JobKey("StadiumImportJob"),
                    null,
                    context.CancellationToken);

                await TriggerAndWaitAsync(
                    scheduler,
                    new JobKey("WeatherForecastImportJob"),
                    null,
                    context.CancellationToken);

                _logger.LogInformation("UTC import pipeline finished");
            }
            finally
            {
                await scheduler.ResumeTrigger(weatherForecastHourlyTriggerKey, context.CancellationToken);
            }
        }

        private async Task TriggerAndWaitAsync(
            IScheduler scheduler,
            JobKey jobKey,
            JobDataMap? jobDataMap,
            CancellationToken cancellationToken)
        {
            var triggerKey = new TriggerKey($"{jobKey.Name}-pipeline-{Guid.NewGuid():N}");

            _logger.LogInformation("Triggering job {JobKey} with trigger {TriggerKey}", jobKey, triggerKey);

            var triggerBuilder = TriggerBuilder.Create()
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                .StartNow();

            if (jobDataMap is not null)
            {
                triggerBuilder = triggerBuilder.UsingJobData(jobDataMap);
            }

            await scheduler.ScheduleJob(triggerBuilder.Build(), cancellationToken);

            await WaitForTriggeredJobToFinishAsync(scheduler, jobKey, triggerKey, cancellationToken);

            _logger.LogInformation("Job {JobKey} finished", jobKey);
        }

        private async Task WaitForTriggeredJobToFinishAsync(
            IScheduler scheduler,
            JobKey jobKey,
            TriggerKey triggerKey,
            CancellationToken cancellationToken)
        {
            var hasStarted = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                var currentlyExecutingJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);
                var isRunning = currentlyExecutingJobs.Any(job =>
                    job.JobDetail.Key == jobKey &&
                    job.Trigger.Key == triggerKey);

                if (isRunning)
                {
                    hasStarted = true;
                }

                if (hasStarted && !isRunning)
                {
                    return;
                }

                if (!hasStarted)
                {
                    var triggerState = await scheduler.GetTriggerState(triggerKey, cancellationToken);

                    if (triggerState == TriggerState.None)
                    {
                        return;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
