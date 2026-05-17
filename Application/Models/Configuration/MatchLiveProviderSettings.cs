namespace Application.Models.Configuration
{
    public class MatchLiveProviderSettings
    {
        public string BaseUrl { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string PrimaryBookmaker { get; set; } = default!;
        public int? RequestsPerMinute { get; set; }
    }
}
