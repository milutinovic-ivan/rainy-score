using Application.Intefraces;
using Application.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.ExternalServices.ApiFootball
{
    public class ApiFootballMatchDetailsCurrentService : IMatchHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiFootballMatchDetailsCurrentService> _logger;

        public ApiFootballMatchDetailsCurrentService(IHttpClientFactory factory, ILogger<ApiFootballMatchDetailsCurrentService> logger)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;
        }

        public async Task<List<MatchHistoryData>> GetMatchHistoriesAsync()
        {
            var url = $"";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error while sending api football request: {url} response: " +
                    $"{response.StatusCode.ToString()}, {response.Content.ToString()}");

                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            return new List<MatchHistoryData> { };
        }
    }
}
