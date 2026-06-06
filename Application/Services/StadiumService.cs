using Application.Intefraces;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly IStadiumInfoService _stadiumInfoService;
        private readonly IGeocodingService _geocodingService;
        private readonly ILogger<StadiumService> _logger;

        public StadiumService(IStadiumInfoService stadiumInfoService, 
            IGeocodingService geocodingService,
            ILogger<StadiumService> logger)
        {
            _stadiumInfoService = stadiumInfoService;
            _geocodingService = geocodingService;
            _logger = logger;
        }

        public async Task<StadiumData?> GetStadiumDataAsync(string teamName, string countryName)
        {
            var stadiumInfo = await _stadiumInfoService.GetStadiumInfoAsync(teamName, countryName);

            string query;

            if (stadiumInfo == null)
            {
                query = $"{teamName} stadium {countryName}";
                _logger.LogDebug($"Geocoding query created pattern, no stadium info: team + 'stadium' + country: {query}");
            }
            //if stadium name and city exists, use it
            else if (!string.IsNullOrWhiteSpace(stadiumInfo.StadiumName) && !string.IsNullOrWhiteSpace(stadiumInfo.City))
            {
                query = $"{stadiumInfo.StadiumName} {stadiumInfo.City} {countryName}";
                _logger.LogDebug($"Geocoding query created pattern: stadiumName + city + country: {query}");
            }
            //if address and city exists, use it
            else if (!string.IsNullOrWhiteSpace(stadiumInfo.Address) && !string.IsNullOrWhiteSpace(stadiumInfo.City))
            {
                query = $"{stadiumInfo.Address} {stadiumInfo.City} {countryName}";
                _logger.LogDebug($"Geocoding query created pattern: address + city + country: {query}");
            }
            else
            {
                query = $"{teamName} stadium {countryName}";
                _logger.LogDebug($"Geocoding query created pattern: team + 'stadium' + country: {query}");
            }

            var coordinates = await _geocodingService.GetCoordinatesAsync(query);

            return new StadiumData
            {
                Name = string.IsNullOrWhiteSpace(stadiumInfo?.StadiumName) ? coordinates?.Name : stadiumInfo.StadiumName,
                TeamName = teamName,
                City = stadiumInfo?.City,
                Address = stadiumInfo?.Address,
                TerrainType = stadiumInfo?.TerrainType,
                Latitude = coordinates?.Latitude,
                Longitude = coordinates?.Longitude
            };
        }
    }
}
