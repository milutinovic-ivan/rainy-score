namespace Infrastructure.ExternalServices.WeatherApi
{

    public class WeatherApiCurrentResponseDto
    {
        public Location location { get; set; }
        public Current current { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string tz_id { get; set; }
        public double localtime_epoch { get; set; }
        public string localtime { get; set; }
    }

    public class Current
    {
        public double last_updated_epoch { get; set; }
        public string last_updated { get; set; }
        public float temp_c { get; set; }
        public float temp_f { get; set; }
        public double is_day { get; set; }
        public Condition condition { get; set; }
        public float wind_mph { get; set; }
        public float wind_kph { get; set; }
        public double wind_degree { get; set; }
        public string wind_dir { get; set; }
        public double pressure_mb { get; set; }
        public float pressure_in { get; set; }
        public double precip_mm { get; set; }
        public double precip_in { get; set; }
        public double humidity { get; set; }
        public double cloud { get; set; }
        public double feelslike_c { get; set; }
        public float feelslike_f { get; set; }
        public double vis_km { get; set; }
        public double vis_miles { get; set; }
        public double uv { get; set; }
        public float gust_mph { get; set; }
        public float gust_kph { get; set; }
        public Air_Quality air_quality { get; set; }
    }

    public class Condition
    {
        public string text { get; set; }
        public string icon { get; set; }
        public double code { get; set; }
    }

    public class Air_Quality
    {
        public float co { get; set; }
        public float no2 { get; set; }
        public float o3 { get; set; }
        public double so2 { get; set; }
        public float pm2_5 { get; set; }
        public double pm10 { get; set; }
        public double usepaindex { get; set; }
        public double gbdefraindex { get; set; }
    }
}
