using Application.Intefraces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Jobs
{
    public class WeatherHistoryImportJob : IJob
    {
        const int MAX_SERVICE_REQUESTS_PER_EXECUTION = 4000;

        private readonly ILogger<WeatherHistoryImportJob> _logger;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;
        private readonly IWeatherHistoryService _weatherHistoryService;
        private readonly IUnitOfWork _unitOfWork;

        public WeatherHistoryImportJob(ILogger<WeatherHistoryImportJob> logger, 
            IRepository<MatchDetails> matchDetailsRepository,
            IWeatherHistoryService weatherHistoryService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _matchDetailsRepository = matchDetailsRepository;
            _weatherHistoryService = weatherHistoryService;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Job started...");

            int serviceRequestCount = 0;
            int matchProcessedCount = 0;
            int skippedNoStadium = 0;
            int skippedResponseNull = 0;

            var allMatchDetails = await _matchDetailsRepository.GetAllAsync(q => q.
                Where(md => md.IsHistory).
                Include(md => md.HomeTeam).
                    ThenInclude(ht => ht.Stadium).
                Include(md => md.WeatherCondition));

            foreach(var match in allMatchDetails)
            {
                string? originalResponse = null;

                if(match.HomeTeam.Stadium == null)
                {
                    _logger.LogWarning($"Match details id: {match.Id} has no home team stadium information. Skipping...");
                    skippedNoStadium++;

                    continue;
                }

                //get weather conditions response just if not already exists
                if (match.WeatherCondition == null)
                {
                    if(match.HomeTeam.Stadium.Latitude.HasValue && match.HomeTeam.Stadium.Longitude.HasValue)
                    {
                        if(serviceRequestCount >= MAX_SERVICE_REQUESTS_PER_EXECUTION)
                        {
                            _logger.LogInformation($"Limit of {serviceRequestCount} service request reached.");
                            break;
                        }

                        originalResponse = await _weatherHistoryService.GetWeatherHistoryResponseAsync(match.HomeTeam.Stadium.Latitude.Value, 
                            match.HomeTeam.Stadium.Longitude.Value, match.MatchDate);

                        serviceRequestCount++;
                    }
                }
                else
                {
                    originalResponse = match.WeatherCondition.OriginalResponse;
                }


                if (originalResponse == null)
                {
                    _logger.LogError($"For match details id: {match.Id} original weather response is null");
                    skippedResponseNull++;

                    continue;
                }

                //pharse data anyway and insert or update object, bacause maybe pharsing logic is changed
                var weatherConditionsData = _weatherHistoryService.PharseWeatherHistoryResponse(originalResponse, match.MatchTime);

                match.WeatherCondition ??= new WeatherCondition
                {
                    MatchDetailsId = match.Id
                };

                match.WeatherCondition.Temperature2m = weatherConditionsData.Temperature2m;
                match.WeatherCondition.DewPoint2m = weatherConditionsData.DewPoint2m;
                match.WeatherCondition.Precipitation = weatherConditionsData.Precipitation;
                match.WeatherCondition.CloudCover = weatherConditionsData.CloudCover;
                match.WeatherCondition.CloudCoverLow = weatherConditionsData.CloudCoverLow;
                match.WeatherCondition.WindSpeed10m = weatherConditionsData.WindSpeed10m;
                match.WeatherCondition.SunshineDuration = weatherConditionsData.SunshineDuration;
                match.WeatherCondition.WeatherCode = weatherConditionsData.WeatherCode;
                match.WeatherCondition.WeatherServiceCode = weatherConditionsData.WeatherServiceCode;
                match.WeatherCondition.OriginalResponse = weatherConditionsData.OriginalResponse;

                matchProcessedCount++;
            }

            // save all at once
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Job finished... Match Processed Count: {matchProcessedCount}, " +
                $" Service Request Count: {serviceRequestCount}" +
                $" Skipped no stadium count: {skippedNoStadium}" +
                $" Skipped service response null: {skippedResponseNull}");
        }
    }
}
