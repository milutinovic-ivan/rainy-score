using Application.Intefraces;
using Application.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<StadiumData?> GetStadiumDataAsync(string teamName)
        {
            var stadiumInfo = await _stadiumInfoService.GetStadiumInfoAsync(teamName);

            string query;

            if (stadiumInfo == null)
            {
                query = $"{teamName} stadium";
                _logger.LogDebug($"Geocoding query created pattern: TeamName + stadium: {query}");
            }
            //if stadium name and city exists, use it
            else if (!string.IsNullOrWhiteSpace(stadiumInfo.StadiumName) && !string.IsNullOrWhiteSpace(stadiumInfo.City))
            {
                query = $"{stadiumInfo.StadiumName} {stadiumInfo.City}";
                _logger.LogDebug($"Geocoding query created pattern: StadiumName + City: {query}");
            }
            //if address and city exists, use it
            else if (!string.IsNullOrWhiteSpace(stadiumInfo.Address) && !string.IsNullOrWhiteSpace(stadiumInfo.City))
            {
                query = $"{stadiumInfo.Address} {stadiumInfo.City}";
                _logger.LogDebug($"Geocoding query created pattern: Address + City: {query}");
            }
            else
            {
                query = $"{teamName} stadium";
                _logger.LogDebug($"Geocoding query created pattern: TeamName + stadium: {query}");
            }

            var coordinates = await _geocodingService.GetCoordinatesAsync(query);

            return new StadiumData
            {
                Name = stadiumInfo?.StadiumName,
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
