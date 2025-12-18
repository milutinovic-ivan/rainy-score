using Application.Intefraces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Stadium.GooglePlaceApi
{
    public class GooglePlaceStadiumLocationService : IStadiumLocationService
    {
        HttpClient _httpClient;

        const string URL = "https://places.googleapis.com/v1/places:searchText";
        const string API_KEY = "AIzaSyA_3dslJZNTmzAxKeigy717v2lbIRqKyxo";
        private readonly ILogger<GooglePlaceStadiumLocationService> _logger;

        public GooglePlaceStadiumLocationService(IHttpClientFactory factory, ILogger<GooglePlaceStadiumLocationService> logger)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;
        }

        public async Task<(decimal? latitude, decimal? longitude)> GetStadiumLocationAsync(string stadiumName)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, URL);
            // Required headers
            req.Headers.Add("X-Goog-Api-Key", API_KEY);
            req.Headers.Add("X-Goog-FieldMask", "places.displayName,places.location,places.types");

            req.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { textQuery = $"{stadiumName} stadium" }),
                Encoding.UTF8,
                "application/json");

            // Send request
            using var response = await _httpClient.SendAsync(req);

            // Throw if not 2xx
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while sending google place api request: {req.Content.ToString} {response.StatusCode.ToString()}, {response.Content.ToString}");
                return (null, null);
            }

            // Read response
            var responseJson = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var stadiumResponse = JsonSerializer.Deserialize<StadiumResponse>(responseJson, options);

            return GetBestCordinates(stadiumResponse);
        }

        private (decimal? latitude, decimal? longitude) GetBestCordinates(StadiumResponse? stadiumResponse)
        {
            //no places found
            if (stadiumResponse?.Places == null || stadiumResponse.Places.Count == 0)
            {
                return (null, null);
            }

            //pick first place whith stadium type, or just take first place
            var stadiumPlace = stadiumResponse.Places.FirstOrDefault(p => p.Types?.Contains("stadium") == true)
                ?? stadiumResponse.Places.First();

            return stadiumPlace.Location != null ? (stadiumPlace.Location.Latitude, stadiumPlace.Location.Longitude) : (null, null); 
        }

        private class StadiumResponse
        {
            public List<Place> Places { get; set; }
        }

        private class Place
        {
            public List<string> Types { get; set; }
            public Location Location { get; set; }
            public DisplayName DisplayName { get; set; }
        }

        private class Location
        {
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        }

        private class DisplayName
        {
            public string Text { get; set; }
            public string LanguageCode { get; set; }
        }
    }
}
