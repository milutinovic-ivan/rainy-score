using Application.Intefraces;
using Application.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.ExternalServices.MatchLive.ApiFootball
{
    public class ApiFootballMatchLiveService : IMatchLiveService, ILeagueService
    {
        const string BASE_URL = "https://v3.football.api-sports.io";
        const string API_KEY = "33ce4dc10403efcdb4b51782b8e7d73c";

        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiFootballMatchLiveService> _logger;

        public ApiFootballMatchLiveService(IHttpClientFactory factory, ILogger<ApiFootballMatchLiveService> logger)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;
        }

        public async Task<List<MatchDetailsData>> GetMatchDetailsListAsync(DateOnly matchDate)
        {
            var jsonString = await GetFixturesJsonAsync(matchDate);
            var matchDetailsDataList = ParseFixtures(jsonString);

            return matchDetailsDataList;
        }

        public async Task<LeagueData?> GetLeagueData(int leagueId)
        {
            var jsonString = await GetLeagueJsonAsync(leagueId);
            var leagueData = ParseLeague(jsonString);

            return leagueData;
        }

        private async Task<string?> GetFixturesJsonAsync(DateOnly matchDate)
        {
            var url = $"{BASE_URL}/fixtures?date={matchDate:yyyy-MM-dd}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-apisports-key", API_KEY);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "API-Football error. Url: {Url}, Status: {Status}, Body: {Body}",
                    url, response.StatusCode, error);

                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }

        private List<MatchDetailsData> ParseFixtures(string? jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return new List<MatchDetailsData>();
            }

            //dispose rootDoc 
            using var rootDoc = JsonDocument.Parse(jsonString);

            if (!rootDoc.RootElement.TryGetProperty("response", out var responseEl) || responseEl.ValueKind != JsonValueKind.Array)
            {
                return new List<MatchDetailsData>();
            }

            var result = new List<MatchDetailsData>();

            foreach (var item in responseEl.EnumerateArray())
            {
                var matchDetails = new MatchDetailsData();

                //fixture
                var dateStr = item.GetProperty("fixture").GetProperty("date").GetString();
                var timeStr = item.GetProperty("fixture").GetProperty("timestamp").GetInt64();
                if (string.IsNullOrWhiteSpace(dateStr))
                {
                    continue;
                }

                matchDetails.FixtureId = item.GetProperty("fixture").GetProperty("id").GetInt32();
                matchDetails.Status = item.GetProperty("fixture").GetProperty("status").GetProperty("short").GetString();
                matchDetails.MatchDate = DateOnly.FromDateTime(DateTimeOffset.Parse(dateStr).UtcDateTime);
                matchDetails.MatchTime = TimeOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(timeStr).UtcDateTime);

                //league
                var leagueEl = item.GetProperty("league");

                matchDetails.LeagueId = leagueEl.GetProperty("id").GetInt32();
                matchDetails.LeagueName = leagueEl.GetProperty("name").GetString();
                matchDetails.Country = leagueEl.GetProperty("country").GetString();

                //teams
                var teamsEl = item.GetProperty("teams");

                matchDetails.HomeTeam = teamsEl.GetProperty("home").GetProperty("name").GetString() ?? string.Empty;
                matchDetails.AwayTeam = teamsEl.GetProperty("away").GetProperty("name").GetString() ?? string.Empty;

                //score
                if(matchDetails.Status == "FT")
                {
                    var fullTimeEl = item.GetProperty("score").GetProperty("fulltime");

                    matchDetails.FullTimeHomeGoals = fullTimeEl.GetProperty("home").GetInt32();
                    matchDetails.FullTimeAwayGoals = fullTimeEl.GetProperty("away").GetInt32();
                    matchDetails.FullTimeWiner = matchDetails.FullTimeHomeGoals > matchDetails.FullTimeAwayGoals ? 'H' 
                        : matchDetails.FullTimeAwayGoals > matchDetails.FullTimeHomeGoals ? 'A' : 'D';

                    var halfTimeEl = item.GetProperty("score").GetProperty("halftime");

                    matchDetails.HalfTimeHomeGoals = halfTimeEl.GetProperty("home").GetInt32();
                    matchDetails.HalfTimeAwayGoals = halfTimeEl.GetProperty("away").GetInt32();
                    matchDetails.HalfTimeWiner = matchDetails.HalfTimeHomeGoals > matchDetails.HalfTimeAwayGoals ? 'H' 
                        : matchDetails.HalfTimeAwayGoals > matchDetails.HalfTimeHomeGoals ? 'A' : 'D';
                }

                matchDetails.IsHistory = false;
                matchDetails.DataSource = "ApiFootball";

                result.Add(matchDetails);
            }

            return result;
        }

        private async Task<string?> GetLeagueJsonAsync(int leagueId)
        {
            var url = $"{BASE_URL}/leagues?id={leagueId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-apisports-key", API_KEY);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "API-Football error. Url: {Url}, Status: {Status}, Body: {Body}",
                    url, response.StatusCode, error);

                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }

        private LeagueData? ParseLeague(string? jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            //dispose rootDoc 
            using var rootDoc = JsonDocument.Parse(jsonString);

            if (!rootDoc.RootElement.TryGetProperty("response", out var responseEl))
            {
                return null;
            }

            var leagueData = new LeagueData();

            leagueData.Id = responseEl.GetProperty("league").GetProperty("id").GetInt32();
            leagueData.Name = responseEl.GetProperty("league").GetProperty("name").GetString();
            leagueData.Country = responseEl.GetProperty("country").GetProperty("name").GetString();
            leagueData.IsCup = responseEl.GetProperty("league").GetProperty("type").GetString() != "League";

            return leagueData;
        }
    }
}
