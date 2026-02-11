using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Application.Models
{
    public class MatchDetailsData
    {
        public string? DataSource { get; set; }
        public int? ExternalLeagueId { get; set; }
        public bool? IsCup { get; set; }
        public int? FixtureId { get; set; }
        public string? LeagueName { get; set; }
        public string? Country { get; set; }
        public string? LeagueDivision { get; set; }
        public DateOnly MatchDate { get; set; }
        public TimeOnly MatchTime { get; set; }
        public string HomeTeam { get; set; } = null!;
        public string AwayTeam { get; set; } = null!;
        public int? FullTimeHomeGoals { get; set; }
        public int? FullTimeAwayGoals { get; set; }
        public char? FullTimeWiner { get; set; }
        public int? HalfTimeHomeGoals { get; set; }
        public int? HalfTimeAwayGoals { get; set; }
        public char? HalfTimeWiner { get; set; }
        public decimal? HomeWinOdds { get; set; }
        public decimal? DrawWinOdds { get; set; }
        public decimal? AwayWinOdds { get; set; }
        public decimal? GoalsOver25Odds {get; set;}
        public decimal? GoalsUnder25Odds {get; set;}
        public bool IsHistory { get; set; }
        public string? Status { get; set; }
        public int? BookmakerId { get; set; }
        public string? BookmakerName { get; set; }
        public JsonDocument? OriginalResponseOdds { get; set; }
    }
}
