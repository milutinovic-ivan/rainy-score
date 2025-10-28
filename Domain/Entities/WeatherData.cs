namespace Domain.Entities
{
    public class WeatherData
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string LocalTime { get; set; }
        public string LastUpdated { get; set; }
        public float TempC { get; set; }
        public bool IsRain {  get; set; }
        public bool IsSnow {  get; set; }
        public float WindKph { get; set; }
        public double PressureMb { get; set; }
        public double PrecipMm { get; set; }
        public double Humidity { get; set; }
        public double Cloud { get; set; }
    }
}

