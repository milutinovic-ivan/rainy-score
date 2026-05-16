using Application.Intefraces;
using Application.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.ExternalServices.WeatherForecast.OpenMeteo
{
    public class OpenMeteoWeatherForecastService : IWeatherForecastService
    {
        //TODO put this in config
        const int ANALYZE_LAST_HOURS = 2;
        const string SERVICE_CODE = "OpenMeteoForecast";

        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenMeteoWeatherForecastService> _logger;

        public OpenMeteoWeatherForecastService(IHttpClientFactory factory, ILogger<OpenMeteoWeatherForecastService> logger)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;
        }

        public async Task<string?> GetWeatherForecastResponseAsync(decimal latitude, decimal longitude, DateOnly date)
        {
            string stringDate = date.ToString("yyyy-MM-dd");

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&start_date={stringDate}&end_date={stringDate}" +
                $"&hourly=temperature_2m,dew_point_2m,precipitation,cloud_cover,cloud_cover_low,wind_speed_10m,sunshine_duration,weather_code";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while sending open meteo request: {url} response: " +
                    $"{response.StatusCode.ToString()}, {response.Content.ToString()}");

                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public WeatherConditionsData PharseWeatherForecastResponse(string response, TimeOnly time)
        {
            //how long before and after match start we summarize rain or snow, shoud be more exact with minutes here
            int hourFrom = time.Hour - ANALYZE_LAST_HOURS;
            //not calculate hour when match is finished or almost finished 
            int hourTo = time.Minute < 30 ? time.Hour + 1 : time.Hour + 2;
            //based on minute calculate real match during hour
            int hourDuring = time.Minute < 30 ? time.Hour : time.Hour + 1;

            var openMeteoResponse = JsonSerializer.Deserialize<OpenMeteoForecastResponse>(response);

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
            weatherConditionsData.WeatherServiceCode = SERVICE_CODE;
            weatherConditionsData.OriginalResponse = response;

            return weatherConditionsData;
        }
    }
}
