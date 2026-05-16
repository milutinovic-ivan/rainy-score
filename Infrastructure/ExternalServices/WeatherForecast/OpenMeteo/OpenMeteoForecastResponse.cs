namespace Infrastructure.ExternalServices.WeatherForecast.OpenMeteo
{
    public class OpenMeteoForecastResponse
    {
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public decimal generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public decimal elevation { get; set; }
        public Hourly_Units hourly_units { get; set; }
        public Hourly hourly { get; set; }
    }

    public class Hourly_Units
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
        public string dew_point_2m { get; set; }
        public string precipitation { get; set; }
        public string cloud_cover { get; set; }
        public string cloud_cover_low { get; set; }
        public string wind_speed_10m { get; set; }
        public string sunshine_duration { get; set; }
        public string weather_code { get; set; }
    }

    public class Hourly
    {
        public string[] time { get; set; }
        public decimal[] temperature_2m { get; set; }
        public decimal[] dew_point_2m { get; set; }
        public decimal[] precipitation { get; set; }
        public int[] cloud_cover { get; set; }
        public int[] cloud_cover_low { get; set; }
        public decimal[] wind_speed_10m { get; set; }
        public decimal[] sunshine_duration { get; set; }
        public int[] weather_code { get; set; }
    }
}
