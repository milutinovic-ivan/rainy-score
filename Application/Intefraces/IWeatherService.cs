using Domain.Entities;

namespace Application.Intefraces
{
    public interface IWeatherService
    {
        Task<WeatherData> GetCurrentWeatherAsync(double lat, double lon);
    }
}
