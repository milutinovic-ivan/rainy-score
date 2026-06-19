using Application.Intefraces;
using Application.Models;
using Application.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.ExternalServices.WeatherHistory.OpenMeteo
{
    public class OpenMeteoWeatherHistoryService : IWeatherHistoryService
    {
        private const string ServiceName = "OpenMeteo";

        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenMeteoWeatherHistoryService> _logger;
        private readonly WeatherProviderSettings _providerSettings;

        public OpenMeteoWeatherHistoryService(IHttpClientFactory factory, 
            ILogger<OpenMeteoWeatherHistoryService> logger,
            IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;

            _providerSettings = configuration
                .GetSection($"Weather:History:{ServiceName}")
                .Get<WeatherProviderSettings>()
                ?? throw new Exception("Missing weather provider config");
        }

        public async Task<string?> GetWeatherHistoryResponseAsync(decimal latitude, decimal longitude, DateOnly date)
        {
            await ApplyDelayIfNeeded();

            string stringDate = date.ToString("yyyy-MM-dd");

            var url = $"{_providerSettings.BaseUrl}?latitude={latitude}&longitude={longitude}&start_date={stringDate}&end_date={stringDate}" +
                $"&hourly=temperature_2m,dew_point_2m,precipitation,cloud_cover,cloud_cover_low,wind_speed_10m,sunshine_duration,weather_code";

            var response = await _httpClient.GetAsync(url);
            
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while sending open meteo request: {url} response: " +
                    $"{response.StatusCode.ToString()}, {response.Content.ToString()}");

                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public WeatherConditionsData PharseWeatherHistoryResponse(string response, TimeOnly time)
        {
            //how long before and after match start we summarize rain or snow, shoud be more exact with minutes here
            int hourFrom = time.Hour - _providerSettings.AnalyzeLastHours;
            //not calculate hour when match is finished or almost finished 
            int hourTo = time.Minute < 30 ? time.Hour + 1 : time.Hour + 2;
            //based on minute calculate real match during hour
            int hourDuring = time.Minute < 30 ? time.Hour : time.Hour + 1;

            var openMeteoResponse = JsonSerializer.Deserialize<OpenMeteoHistoryResponse>(response);

            var weatherConditionsData = new WeatherConditionsData();

            weatherConditionsData.Latitude = openMeteoResponse.latitude;
            weatherConditionsData.Longitude = openMeteoResponse.longitude;
            weatherConditionsData.Temperature2m = openMeteoResponse.hourly.temperature_2m[hourDuring];
            weatherConditionsData.DewPoint2m = openMeteoResponse.hourly.dew_point_2m[hourDuring];
            weatherConditionsData.Precipitation = openMeteoResponse.hourly.precipitation.Skip(hourFrom).Take(hourTo - hourFrom + 1).Sum();
            weatherConditionsData.CloudCover = openMeteoResponse.hourly.cloud_cover[hourDuring];
            weatherConditionsData.CloudCoverLow = openMeteoResponse.hourly.cloud_cover_low[hourDuring];
            weatherConditionsData.WindSpeed10m = openMeteoResponse.hourly.wind_speed_10m[hourDuring];
            weatherConditionsData.WeatherCode = openMeteoResponse.hourly.weather_code[hourDuring];
            weatherConditionsData.WeatherServiceCode = ServiceName;
            weatherConditionsData.OriginalResponse = response;

            return weatherConditionsData;
        }

        private async Task ApplyDelayIfNeeded()
        {
            //calculate delay, if needed
            if (_providerSettings.RequestsPerMinute.HasValue)
            {
                var delay = TimeSpan.FromMinutes(1.0 / _providerSettings.RequestsPerMinute.Value);
                _logger.LogDebug($"Applying delay of {delay.TotalSeconds} seconds to respect rate limits");

                await Task.Delay(delay);
            }
        }
    }
}
