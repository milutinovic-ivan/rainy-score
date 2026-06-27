using Application.Jobs.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class UtcImportPipelineJob : IJob
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<UtcImportPipelineJob> _logger;
        private readonly IJobExecutionsService _jobExecutionsService;

        public UtcImportPipelineJob(
            ISchedulerFactory schedulerFactory,
            ILogger<UtcImportPipelineJob> logger,
            IJobExecutionsService jobExecutionsService)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
            _jobExecutionsService = jobExecutionsService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);
            var weatherForecastHourlyTriggerKey = new TriggerKey("WeatherForecastImportJobHourlyTrigger");

            var executionId = await _jobExecutionsService.StartAsync(nameof(UtcImportPipelineJob));

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

                await _jobExecutionsService.FinishAsync(executionId, JobExecutionStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during job execution");
                await _jobExecutionsService.FinishAsync(executionId, JobExecutionStatus.Failed, null, ex);

                throw;
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
