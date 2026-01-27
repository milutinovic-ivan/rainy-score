using Application.Intefraces;
using Application.Models;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Stadium.GooglePlaceApi
{
    public class GooglePlaceStadiumService : IStadiumService
    {
        HttpClient _httpClient;

        const string URL = "https://places.googleapis.com/v1/places:searchText";
        const string API_KEY = "AIzaSyA_3dslJZNTmzAxKeigy717v2lbIRqKyxo";
        private readonly ILogger<GooglePlaceStadiumService> _logger;


        public GooglePlaceStadiumService(IHttpClientFactory factory, ILogger<GooglePlaceStadiumService> logger)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;
        }

        public async Task<StadiumData?> GetStadiumDataAsync(string teamName)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, URL);
            // Required headers
            req.Headers.Add("X-Goog-Api-Key", API_KEY);
            req.Headers.Add("X-Goog-FieldMask", "places.displayName,places.location,places.types");

            req.Content = new StringContent(
                JsonSerializer.Serialize(new { textQuery = $"{teamName} stadium" }),
                Encoding.UTF8,
                "application/json");

            // Send request
            using var response = await _httpClient.SendAsync(req);

            // Throw if not 2xx
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while sending google place api request: {req.Content.ToString} response: " +
                    $"{response.StatusCode.ToString()}, {response.Content.ToString()}");

                return null;
            }

            // Read response
            var responseJson = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var stadiumResponse = JsonSerializer.Deserialize<StadiumResponse>(responseJson, options);

            return GetBestStadiumMatch(stadiumResponse);
        }

        private StadiumData? GetBestStadiumMatch(StadiumResponse? stadiumResponse)
        {
            //no places found
            if (stadiumResponse?.Places == null || stadiumResponse.Places.Count == 0)
            {
                return null;
            }

            //pick first place whith stadium type, or just take first place
            var stadiumPlace = stadiumResponse.Places.FirstOrDefault(p => p.Types?.Contains("stadium") == true)
                ?? stadiumResponse.Places.First();

            if(stadiumPlace.Location != null && stadiumPlace.DisplayName != null)
            {
                return new StadiumData
                {
                    Name = stadiumPlace.DisplayName.Text,
                    Latitude = stadiumPlace.Location.Latitude,
                    Longitude = stadiumPlace.Location.Longitude
                };
            }
            else
            {
                return null;
            }
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
