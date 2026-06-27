using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Jobs.Services
{
    internal class JobExecutionsService : IJobExecutionsService
    {
        private readonly IRepository<JobExecution> _jobExecutionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<JobExecutionsService> _logger;

        public JobExecutionsService(IRepository<JobExecution> jobExecutionRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<JobExecutionsService> logger)
        {
            _jobExecutionRepository = jobExecutionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<int> StartAsync(string jobName)
        {
            var jobExecution = new JobExecution
            {
                JobName = jobName,
                StartTime = DateTime.UtcNow,
                Status = JobExecutionStatus.Running.ToString().ToLower()
            };

            await _jobExecutionRepository.AddAsync(jobExecution);
            await _unitOfWork.SaveChangesAsync();

            return jobExecution.Id;
        }

        public async Task FinishAsync(int executionId, JobExecutionStatus status, object? metricsJson = null, Exception? exception = null)
        {
            var jobExecution = await _jobExecutionRepository.GetByIdAsync(executionId);

            if(jobExecution == null)
            {
                _logger.LogError($"Job execution with ID {executionId} not found.");
                return;
            }

            jobExecution.EndTime = DateTime.UtcNow;
            jobExecution.Status = status.ToString().ToLower();
            jobExecution.MetricsJson = metricsJson != null ? JsonSerializer.Serialize(metricsJson) : null;
            jobExecution.HasError = exception != null;
            jobExecution.ErrorMessage = exception?.Message;

            await _unitOfWork.SaveChangesAsync();
        }
    }

    public enum JobExecutionStatus
    {
        Running,
        Success,
        Failed
    }
}
