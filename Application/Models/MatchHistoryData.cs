using System.IO;
using System.Text.RegularExpressions;

namespace Application.Models
{
    public class MatchHistoryData
    {
        public string LeagueDivision { get; set; }
        public string MatchDate { get; set; }
        public string MatchTime { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int FullTimeHomeGoals { get; set; }
        public int FullTimeAwayGoals { get; set; }
        public char FullTimeWiner { get; set; }
        public int HalfTimeHomeGoals { get; set; }
        public int HalfTimeAwayGoals { get; set; }
        public char HalfTimeWiner { get; set; }
        public int HomeShots { get; set; }
        public int AwayShots { get; set; }
        public int HomeShotsOnTarget { get; set; }
        public int AwayShotsOnTarget { get; set; }
        public decimal HomeWinOdds { get; set; }
        public decimal DrawWinOdds { get; set; }
        public decimal AwayWinOdds { get; set; }
        public decimal GoalsOver25 {get; set;}
        public decimal GoalsUnder25 {get; set;}
    }
}
