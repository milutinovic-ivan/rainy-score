using Application.Intefraces;
using Application.Models;
using Application.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Infrastructure.ExternalServices.Stadium.GooglePlaceApi
{
    public class GooglePlaceGeocodingService : IGeocodingService
    {
        private const string ServiceName = "googleplace";

        private readonly HttpClient _httpClient;
        private readonly ILogger<GooglePlaceGeocodingService> _logger;
        private readonly GeocodingProviderSettings _providerSettings;

        public GooglePlaceGeocodingService(IHttpClientFactory factory, 
            ILogger<GooglePlaceGeocodingService> logger, 
            IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;

            _providerSettings = configuration
                .GetSection($"GeocodingProviders:{ServiceName}")
                .Get<GeocodingProviderSettings>()
                ?? throw new Exception("Missing geocoding provider config");
        }

        public async Task<Coordinates?> GetCoordinatesAsync(string query)
        {
            await ApplyDelayIfNeeded();

            using var req = new HttpRequestMessage(HttpMethod.Post, _providerSettings.BaseUrl);
            // Required headers
            req.Headers.Add("X-Goog-Api-Key", _providerSettings.ApiKey);
            req.Headers.Add("X-Goog-FieldMask", "places.displayName,places.location,places.types");

            req.Content = new StringContent(
                JsonSerializer.Serialize(new { textQuery = query }),
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

        private Coordinates? GetBestStadiumMatch(StadiumResponse? stadiumResponse)
        {
            //no places found
            if (stadiumResponse?.Places == null || stadiumResponse.Places.Count == 0)
            {
                return null;
            }

            //pick first place whith stadium type, or just take first place
            var stadiumPlace = stadiumResponse.Places.FirstOrDefault(p => p.Types?.Contains("stadium") == true)
                ?? stadiumResponse.Places.First();

            if(stadiumPlace.Location != null)
            {
                return new Coordinates
                {
                    Name = stadiumPlace.DisplayName?.Text,
                    Latitude = stadiumPlace.Location.Latitude,
                    Longitude = stadiumPlace.Location.Longitude
                };
            }
            else
            {
                return null;
            }
        }

        private async Task ApplyDelayIfNeeded()
        {
            //calculate delay, if needed
            if (_providerSettings.RequestsPerMinute.HasValue)
            {
               var delay = TimeSpan.FromMinutes(1.0 / _providerSettings.RequestsPerMinute.Value);
                _logger.LogDebug($"Applying delay of {delay.TotalSeconds} seconds to respect rate limits");

                await Task.Delay(delay);
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
