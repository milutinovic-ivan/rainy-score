namespace Application.Jobs.Services
{
    public interface IJobExecutionsService
    {
        Task<int> StartAsync(string jobName);
        Task FinishAsync(int executionId, JobExecutionStatus status, object? metricsJson = null, Exception? exception = null);
    }
}
