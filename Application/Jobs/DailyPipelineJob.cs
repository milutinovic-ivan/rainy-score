using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Listener;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class DailyPipelineJob : IJob
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<DailyPipelineJob> _logger;

        public DailyPipelineJob(
            ISchedulerFactory schedulerFactory,
            ILogger<DailyPipelineJob> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);
            var weatherForecastHourlyTriggerKey = new TriggerKey("WeatherForecastImportJobHourlyTrigger");

            _logger.LogInformation("Daily pipeline started");

            await scheduler.PauseTrigger(weatherForecastHourlyTriggerKey, context.CancellationToken);

            try
            {
                await TriggerAndWaitAsync(
                    scheduler,
                    new JobKey("MatchLiveImportJobYesterday"),
                    null,
                    context.CancellationToken);

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

                _logger.LogInformation("Daily pipeline finished");
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
            var completion = new TaskCompletionSource<object?>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var listener = new WaitForJobCompletionListener(jobKey, completion);

            scheduler.ListenerManager.AddJobListener(
                listener,
                KeyMatcher<JobKey>.KeyEquals(jobKey));

            try
            {
                _logger.LogInformation("Triggering job {JobKey}", jobKey);

                if (jobDataMap is null)
                {
                    await scheduler.TriggerJob(jobKey, cancellationToken);
                }
                else
                {
                    await scheduler.TriggerJob(jobKey, jobDataMap, cancellationToken);
                }

                await completion.Task.WaitAsync(cancellationToken);

                _logger.LogInformation("Job {JobKey} finished", jobKey);
            }
            finally
            {
                scheduler.ListenerManager.RemoveJobListener(listener.Name);
            }
        }

        private sealed class WaitForJobCompletionListener : JobListenerSupport
        {
            private readonly JobKey _jobKey;
            private readonly TaskCompletionSource<object?> _completion;

            public WaitForJobCompletionListener(
                JobKey jobKey,
                TaskCompletionSource<object?> completion)
            {
                _jobKey = jobKey;
                _completion = completion;
            }

            public override string Name { get; } = $"WaitForJobCompletion-{Guid.NewGuid()}";

            public override Task JobWasExecuted(
                IJobExecutionContext context,
                JobExecutionException? jobException,
                CancellationToken cancellationToken = default)
            {
                if (context.JobDetail.Key != _jobKey)
                {
                    return Task.CompletedTask;
                }

                if (jobException is null)
                {
                    _completion.TrySetResult(null);
                }
                else
                {
                    _completion.TrySetException(jobException);
                }

                return Task.CompletedTask;
            }
        }
    }
}
