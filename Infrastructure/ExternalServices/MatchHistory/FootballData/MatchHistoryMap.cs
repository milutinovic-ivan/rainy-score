using CsvHelper.Configuration;
using Application.Models;

namespace Infrastructure.ExternalServices.MatchHistory.FootballData
{
    public sealed class MatchHistoryMap : ClassMap<MatchDetailsData>
    {
        public MatchHistoryMap()
        {
            Map(m => m.LeagueDivision).Name("Div");
            Map(m => m.MatchDate).Name("Date").TypeConverterOption.Format("dd/MM/yyyy");
            Map(m => m.MatchTime).Name("Time").TypeConverterOption.Format("HH:mm");
            Map(m => m.HomeTeam).Name("HomeTeam").Convert(args => NormalizeTeamName(args.Row.GetField("HomeTeam")));
            Map(m => m.AwayTeam).Name("AwayTeam").Convert(args => NormalizeTeamName(args.Row.GetField("AwayTeam")));
            Map(m => m.FullTimeHomeGoals).Name("FTHG");
            Map(m => m.FullTimeAwayGoals).Name("FTAG");
            Map(m => m.FullTimeWiner).Name("FTR");
            Map(m => m.HalfTimeHomeGoals).Name("HTHG");
            Map(m => m.HalfTimeAwayGoals).Name("HTAG");
            Map(m => m.HalfTimeWiner).Name("HTR");
            Map(m => m.HomeWinOdds).Name("AvgCH");
            Map(m => m.DrawWinOdds).Name("AvgCD");
            Map(m => m.AwayWinOdds).Name("AvgCA");
            Map(m => m.GoalsOver25Odds).Name("AvgC>2.5");
            Map(m => m.GoalsUnder25Odds).Name("AvgC<2.5");
        }

        private string NormalizeTeamName(string? input)
        {
            return input?
                .Replace("’", "")   // Right single quotation mark
                .Replace("‘", "")   // Left single quotation mark
                .Replace("�", "")   // Replacement character
            ?? string.Empty;
        }
    }
}
