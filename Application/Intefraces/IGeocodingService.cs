using Application.Models;

namespace Application.Intefraces
{
    public interface IGeocodingService
    {
        Task<Coordinates?> GetCoordinatesAsync(string query);
    }
}
