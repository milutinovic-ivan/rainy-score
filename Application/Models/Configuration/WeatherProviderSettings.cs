namespace Application.Models.Configuration
{
    public class WeatherProviderSettings
    {
        public required string BaseUrl { get; set; }
        public int? RequestsPerMinute { get; set; }
        public required int AnalyzeLastHours { get; set; }
    }
}
