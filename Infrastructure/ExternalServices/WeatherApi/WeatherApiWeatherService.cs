using Application.Intefraces;
using Domain.Entities;
using System.Net.Http;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ExternalServices.WeatherApi
{
    public class WeatherApiWeatherService : IWeatherService
    {
        private const string BASE_URL = "http://api.weatherapi.com/v1/forecast.json";
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public WeatherApiWeatherService(IConfiguration configuration, IHttpClientFactory factory, IMapper mapper)
        {
            _configuration = configuration;
            _httpClient = factory.CreateClient();
            _mapper = mapper;
        }

        public async Task<WeatherData> GetCurrentWeatherAsync(double lat, double lon)
        {
            var apiKey = _configuration["Weather:WeatherApi:ApiKey"];
            var url = $"{BASE_URL}?key={apiKey}&q={lat},{lon}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var weatherApiCurrentResponseDto = JsonSerializer.Deserialize<WeatherApiCurrentResponseDto>(content);

            var weatherData = _mapper.Map<WeatherData>(weatherApiCurrentResponseDto);
            return weatherData;
        }
    }
}
