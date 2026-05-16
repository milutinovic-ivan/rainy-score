using Application.Models;

namespace Application.Intefraces
{
    public interface IWeatherForecastService
    {
        Task<string?> GetWeatherForecastResponseAsync(decimal latitude, decimal longitude, DateOnly date);
        WeatherConditionsData PharseWeatherForecastResponse(string response, TimeOnly time);
    }
}
