using Application.Intefraces;
using Application.Models;
using Domain.Entities;
using Infrastructure.ExternalServices.WeatherApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Weather.OpenMeteo
{
    internal class OpenMeteoWeatherService : IWeatherHistoryService
    {
        //TODO put this in config
        const int ANALYZE_LAST_HOURS = 3;
        const string SERVICE_CODE = "OpenMeteo";

        HttpClient _httpClient;
        public OpenMeteoWeatherService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public async Task<string> GetWeatherHistoryResponseAsync(decimal latitude, decimal longitude, DateOnly date)
        {
            string stringDate = date.ToString("yyyy-MM-dd");

            var url = $"https://archive-api.open-meteo.com/v1/archive?latitude={latitude}&longitude={longitude}&start_date={stringDate}&end_date={stringDate}" +
                $"&hourly=temperature_2m,dew_point_2m,apparent_temperature,surface_pressure,precipitation,rain,snowfall,cloud_cover,cloud_cover_low,et0_fao_evapotranspiration,wind_speed_10m,sunshine_duration,weather_code";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public WeatherConditionsData PharseWeatherHistoryResponse(string response, TimeOnly time)
        {
            //how long before and after match start we summarize rain or snow, shoud be more exact with minutes here
            int hourFrom = time.Hour - ANALYZE_LAST_HOURS;
            int hourTo = time.Hour + 2;
            int hourDuring = time.Hour + 1;

            var openMeteoResponse = JsonSerializer.Deserialize<OpenMeteoResponse>(response);

            var weatherConditionsData = new WeatherConditionsData();

            weatherConditionsData.Latitude = openMeteoResponse.latitude;
            weatherConditionsData.Longitude = openMeteoResponse.longitude;
            weatherConditionsData.Temperature2m = openMeteoResponse.hourly.temperature_2m[hourDuring];
            weatherConditionsData.DewPoint2m = openMeteoResponse.hourly.dew_point_2m[hourDuring];
            weatherConditionsData.ApparentTemperature = openMeteoResponse.hourly.apparent_temperature[hourDuring];
            weatherConditionsData.SurfacePressure = openMeteoResponse.hourly.surface_pressure[hourDuring];
            weatherConditionsData.Precipitation = openMeteoResponse.hourly.precipitation.Skip(hourFrom).Take(hourTo).Sum();
            weatherConditionsData.Rain = openMeteoResponse.hourly.rain.Skip(hourFrom).Take(hourTo).Sum();
            weatherConditionsData.Snowfall = openMeteoResponse.hourly.snowfall.Skip(hourFrom).Take(hourTo).Sum();
            weatherConditionsData.CloudCover = openMeteoResponse.hourly.cloud_cover[hourDuring];
            weatherConditionsData.CloudCoverLow = openMeteoResponse.hourly.cloud_cover_low[hourDuring];
            weatherConditionsData.Et0FaoEvapotranspiration = openMeteoResponse.hourly.et0_fao_evapotranspiration.Skip(hourFrom).Take(hourTo).Average();
            weatherConditionsData.WindSpeed10m = openMeteoResponse.hourly.wind_speed_10m[hourDuring];
            weatherConditionsData.WeatherCode = openMeteoResponse.hourly.weather_code[hourDuring];
            weatherConditionsData.WeatherServiceCode = SERVICE_CODE;
            weatherConditionsData.OriginalResponse = response;

            return weatherConditionsData;
        }
    }
}
