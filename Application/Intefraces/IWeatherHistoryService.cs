using Application.Models;

namespace Application.Intefraces
{
    public interface IWeatherHistoryService
    {
        Task<string?> GetWeatherHistoryResponseAsync(decimal latitude, decimal longitude, DateOnly date);
        WeatherConditionsData PharseWeatherHistoryResponse(string response, TimeOnly time);
    }
}
