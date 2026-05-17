using Application.Intefraces;
using Application.Models;
using Application.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Infrastructure.ExternalServices.MatchLive.ApiFootball
{
    public class ApiFootballMatchLiveService : IMatchLiveService, ILeagueService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiFootballMatchLiveService> _logger;
        private readonly MatchLiveProviderSettings _providerSettings;

        public ApiFootballMatchLiveService(IHttpClientFactory factory,
            ILogger<ApiFootballMatchLiveService> logger,
            IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;

            _providerSettings = configuration
                .GetSection($"MatchLiveProviders:{ServiceName}")
                .Get<MatchLiveProviderSettings>()
                ?? throw new Exception("Missing match live provider config");
        }

        public string ServiceName => "apifootball";

        public async Task<List<MatchDetailsData>> GetMatchDetailsListAsync(DateOnly matchDate)
        {
            var jsonString = await GetFixturesJsonAsync(matchDate);
            var matchDetailsDataList = ParseFixtures(jsonString);

            return matchDetailsDataList;
        }

        public async Task<MatchDetailsData?> GetMatchOddsAsync(int fixtureId)
        {
            var jsonString = await GetOddsJsonAsync(fixtureId);
            var matchDetailsData = ParseOdds(jsonString);

            return matchDetailsData;
        }

        public async Task<LeagueData?> GetLeagueDataAsync(int leagueId)
        {
            var jsonString = await GetLeagueJsonAsync(leagueId);
            var leagueData = ParseLeague(jsonString);

            return leagueData;
        }

        private async Task<string?> GetFixturesJsonAsync(DateOnly matchDate)
        {
            var url = $"{_providerSettings.BaseUrl}/fixtures?date={matchDate:yyyy-MM-dd}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-apisports-key", _providerSettings.ApiKey);

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

                matchDetails.DataSource = ServiceName;

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

                matchDetails.ExternalLeagueId = leagueEl.GetProperty("id").GetInt32();
                matchDetails.LeagueName = leagueEl.GetProperty("name").GetString()?.Trim();
                matchDetails.Country = leagueEl.GetProperty("country").GetString()?.Trim();

                //teams
                var teamsEl = item.GetProperty("teams");

                matchDetails.HomeTeam = teamsEl.GetProperty("home").GetProperty("name").GetString()?.Trim() ?? string.Empty;
                matchDetails.AwayTeam = teamsEl.GetProperty("away").GetProperty("name").GetString()?.Trim() ?? string.Empty;

                //score
                if(matchDetails.Status == "FT")
                {
                    var fullTimeEl = item.GetProperty("score").GetProperty("fulltime");

                    matchDetails.FullTimeHomeGoals = fullTimeEl.GetProperty("home").ValueKind == JsonValueKind.Number
                        ? fullTimeEl.GetProperty("home").GetInt32() : null;
                    matchDetails.FullTimeAwayGoals = fullTimeEl.GetProperty("away").ValueKind == JsonValueKind.Number
                        ? fullTimeEl.GetProperty("away").GetInt32() : null;

                    if(matchDetails.FullTimeHomeGoals.HasValue && matchDetails.FullTimeAwayGoals.HasValue)
                    {
                        matchDetails.FullTimeWiner = matchDetails.FullTimeHomeGoals > matchDetails.FullTimeAwayGoals ? 'H'
                            : matchDetails.FullTimeAwayGoals > matchDetails.FullTimeHomeGoals ? 'A' : 'D';
                    }
                    else
                    {
                        matchDetails.FullTimeWiner = null;
                    }

                    var halfTimeEl = item.GetProperty("score").GetProperty("halftime");

                    matchDetails.HalfTimeHomeGoals = halfTimeEl.GetProperty("home").ValueKind == JsonValueKind.Number
                        ? halfTimeEl.GetProperty("home").GetInt32() : null;
                    matchDetails.HalfTimeAwayGoals = halfTimeEl.GetProperty("away").ValueKind == JsonValueKind.Number
                        ? halfTimeEl.GetProperty("away").GetInt32() : null;

                    if(matchDetails.HalfTimeHomeGoals.HasValue && matchDetails.HalfTimeAwayGoals.HasValue)
                    {
                        matchDetails.HalfTimeWiner = matchDetails.HalfTimeHomeGoals > matchDetails.HalfTimeAwayGoals ? 'H'
                            : matchDetails.HalfTimeAwayGoals > matchDetails.HalfTimeHomeGoals ? 'A' : 'D';
                    }
                    else
                    {
                        matchDetails.HalfTimeWiner = null;
                    }
                }

                matchDetails.IsHistory = false;

                result.Add(matchDetails);
            }

            return result;
        }

        private async Task<string?> GetOddsJsonAsync(int fixtureId)
        {
            var url = $"{_providerSettings.BaseUrl}/odds?fixture={fixtureId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-apisports-key", _providerSettings.ApiKey);

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

        private MatchDetailsData? ParseOdds(string? jsonString)
        {
            var matchDetails = new MatchDetailsData();

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }

            using var rootDoc = JsonDocument.Parse(jsonString);

            if (!rootDoc.RootElement.TryGetProperty("response", out var responseEl) 
                ||responseEl.ValueKind != JsonValueKind.Array 
                || responseEl.GetArrayLength() == 0)
            {
                return null;
            }

            var item = responseEl[0];

            // FixtureId
            matchDetails.FixtureId = item.GetProperty("fixture").GetProperty("id").GetInt32();

            if (!item.TryGetProperty("bookmakers", out var bookmakersEl) ||
                bookmakersEl.ValueKind != JsonValueKind.Array ||
                bookmakersEl.GetArrayLength() == 0)
            {
                return null;
            }

            // pick PrimaryBookmaker if exists, else first
            JsonElement chosenBookmaker = bookmakersEl[0];

            foreach (var bm in bookmakersEl.EnumerateArray())
            {
                var name = bm.GetProperty("name").GetString();
                if (string.Equals(name, _providerSettings.PrimaryBookmaker, StringComparison.OrdinalIgnoreCase))
                {
                    chosenBookmaker = bm;
                    break;
                }
            }

            matchDetails.BookmakerId = chosenBookmaker.GetProperty("id").GetInt32();
            matchDetails.BookmakerName = chosenBookmaker.GetProperty("name").GetString();
            matchDetails.OriginalResponseOdds = JsonDocument.Parse(chosenBookmaker.GetRawText());

            if (!chosenBookmaker.TryGetProperty("bets", out var betsEl) ||
                betsEl.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var bet in betsEl.EnumerateArray())
            {
                var betName = bet.GetProperty("name").GetString();

                if (!bet.TryGetProperty("values", out var valuesEl) || valuesEl.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                // Match Winner
                if (betName == "Match Winner")
                {
                    foreach (var v in valuesEl.EnumerateArray())
                    {
                        var label = v.GetProperty("value").GetString();
                        var oddStr = v.GetProperty("odd").GetString();

                        if (!decimal.TryParse(oddStr, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var odd))
                        {
                            continue;
                        }

                        if (label == "Home") matchDetails.HomeWinOdds = odd;
                        else if (label == "Draw") matchDetails.DrawWinOdds = odd;
                        else if (label == "Away") matchDetails.AwayWinOdds = odd;
                    }
                }

                // Goals Over/Under
                if (betName == "Goals Over/Under")
                {
                    foreach (var v in valuesEl.EnumerateArray())
                    {
                        var label = v.GetProperty("value").GetString();
                        var oddStr = v.GetProperty("odd").GetString();

                        if (!decimal.TryParse(oddStr, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var odd))
                        {
                            continue;
                        }

                        if (label == "Over 2.5") matchDetails.GoalsOver25Odds = odd;
                        else if (label == "Under 2.5") matchDetails.GoalsUnder25Odds = odd;
                    }
                }
            }

            return matchDetails;
        }

        private async Task<string?> GetLeagueJsonAsync(int leagueId)
        {
            var url = $"{_providerSettings.BaseUrl}/leagues?id={leagueId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-apisports-key", _providerSettings.ApiKey);

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

            try
            {
                leagueData.Id = responseEl[0].GetProperty("league").GetProperty("id").GetInt32();
                leagueData.Name = responseEl[0].GetProperty("league").GetProperty("name").GetString();
                leagueData.Country = responseEl[0].GetProperty("country").GetProperty("name").GetString();
                leagueData.IsCup = responseEl[0].GetProperty("league").GetProperty("type").GetString() != "League";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing league data. Json: {Json}", jsonString);
                return null;
            }


            return leagueData;
        }
    }
}
