namespace Application.Models.Configuration
{
    public class GeocodingProviderSettings
    {
        public required string BaseUrl { get; set; }
        public required string ApiKey { get; set; }
        public int? RequestsPerMinute { get; set; }
    }
}
