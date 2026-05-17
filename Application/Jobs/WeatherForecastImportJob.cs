using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Reflection.Metadata.Ecma335;

namespace Application.Jobs
{
    public class WeatherForecastImportJob : IJob
    {
        const int MAX_SERVICE_REQUESTS_PER_EXECUTION = 2000;

        private readonly ILogger<WeatherForecastImportJob> _logger;
        private readonly IRepository<MatchDetails> _matchDetailsRepository;
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly IUnitOfWork _unitOfWork;

        public WeatherForecastImportJob(ILogger<WeatherForecastImportJob> logger, 
            IRepository<MatchDetails> matchDetailsRepository,
            IWeatherForecastService weatherForecastService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _matchDetailsRepository = matchDetailsRepository;
            _weatherForecastService = weatherForecastService;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Job started...");

            int serviceRequestCount = 0;
            int matchProcessedCount = 0;

            var nowUtc = DateTime.UtcNow;
            var todayUtc = DateOnly.FromDateTime(nowUtc);
            var timeUtc = TimeOnly.FromDateTime(nowUtc);

            var allMatchDetails = await _matchDetailsRepository.GetAllAsync(q => q
                .Where(md => md.MatchDate == todayUtc && md.MatchTime > timeUtc)
                .Include(md => md.HomeTeam)
                    .ThenInclude(ht => ht.Stadium)
                .Include(md => md.WeatherCondition));


            foreach (var match in allMatchDetails)
            {
                try
                {
                    string? originalResponse = null;

                    _logger.LogInformation($"Processing match details FixtureId: {match.FixtureId}, match date: {match.MatchDate}, match time: {match.MatchTime}");

                    //get weather conditions response anyway
                    if (match.HomeTeam.Stadium?.Latitude is not null && match.HomeTeam.Stadium?.Longitude is not null)
                    {
                        if (serviceRequestCount >= MAX_SERVICE_REQUESTS_PER_EXECUTION)
                        {
                            _logger.LogInformation($"Limit of {serviceRequestCount} service request reached.");
                            break;
                        }

                        //make delay between every service call
                        await Task.Delay(TimeSpan.FromMilliseconds(200));

                        originalResponse = await _weatherForecastService.GetWeatherForecastResponseAsync(match.HomeTeam.Stadium.Latitude.Value,
                            match.HomeTeam.Stadium.Longitude.Value, match.MatchDate);

                        serviceRequestCount++;
                    }
                    else
                    {
                        _logger.LogError($"For match details id: {match.Id} no stadium data for team: {match.HomeTeam.Name}");
                        continue;
                    }

                    if (originalResponse == null)
                    {
                        _logger.LogError($"For match details id: {match.Id} original weather response is null");
                        continue;
                    }

                    //pharse data anyway and insert or update object, bacause maybe pharsing logic is changed
                    var weatherConditionsData = _weatherForecastService.PharseWeatherForecastResponse(originalResponse, match.MatchTime);

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
                catch (Exception ex)
                {
                    _logger.LogError($"custom msg: {ex.Message}");
                    continue;
                }
            }

            // save all at once
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Job finished... Match Processed Count: {matchProcessedCount}, Service Request Count: {serviceRequestCount}");
        }
    }
}
