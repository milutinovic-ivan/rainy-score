using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class MatchDetails : BaseEntity
    {
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;
        public int Season { get;set; }
        public DateOnly MatchDate { get; set; }
        public TimeOnly MatchTime { get; set; }
        public int HomeTeamId { get; set; }

        [ForeignKey(nameof(HomeTeamId))]
        [InverseProperty("HomeMatches")]
        public Team HomeTeam { get; set; } = null!;
        public int AwayTeamId { get; set; }

        [ForeignKey(nameof(AwayTeamId))]
        [InverseProperty("AwayMatches")]
        public Team AwayTeam { get; set; } = null!;
        //for now, it can have WeatherCondition just from one service, latter could be many
        public WeatherCondition? WeatherCondition { get; set; }
        public int FullTimeHomeGoals { get; set; }
        public int FullTimeAwayGoals { get; set; }
        public char FullTimeWiner { get; set; }
        public int HalfTimeHomeGoals { get; set; }
        public int HalfTimeAwayGoals { get; set; }
        public char HalfTimeWiner { get; set; }
        public decimal HomeWinOdds { get; set; }
        public decimal DrawWinOdds { get; set; }
        public decimal AwayWinOdds { get; set; }
        public decimal GoalsOver25 { get; set; }
        public decimal GoalsUnder25 { get; set; }
        public bool IsHistory { get; set; }
    }
}
