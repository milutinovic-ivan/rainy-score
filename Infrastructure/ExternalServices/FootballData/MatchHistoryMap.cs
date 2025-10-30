using CsvHelper.Configuration;
using Application.Models;

namespace Infrastructure.ExternalServices.FootballData
{
    public sealed class MatchHistoryMap : ClassMap<MatchHistoryData>
    {
        public MatchHistoryMap()
        {
            Map(m => m.LeagueDivision).Name("Div");
            Map(m => m.MatchDate).Name("Date").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.MatchTime).Name("Time").TypeConverterOption.Format("HH:mm");
            Map(m => m.HomeTeam).Name("HomeTeam");
            Map(m => m.AwayTeam).Name("AwayTeam");
            Map(m => m.FullTimeHomeGoals).Name("FTHG");
            Map(m => m.FullTimeAwayGoals).Name("FTAG");
            Map(m => m.FullTimeWiner).Name("FTR");
            Map(m => m.HalfTimeHomeGoals).Name("HTHG");
            Map(m => m.HalfTimeAwayGoals).Name("HTAG");
            Map(m => m.HalfTimeWiner).Name("HTR");
            Map(m => m.HomeShots).Name("HS");
            Map(m => m.AwayShots).Name("AS");
            Map(m => m.HomeShotsOnTarget).Name("HST");
            Map(m => m.AwayShotsOnTarget).Name("AST");
            Map(m => m.HomeWinOdds).Name("AvgCH");
            Map(m => m.DrawWinOdds).Name("AvgCD");
            Map(m => m.AwayWinOdds).Name("AvgCA");
            Map(m => m.GoalsOver25).Name("AvgC>2.5");
            Map(m => m.GoalsUnder25).Name("AvgC<2.5");
        }
    }
}
