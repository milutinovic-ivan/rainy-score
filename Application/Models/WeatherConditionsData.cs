using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class WeatherConditionsData
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Temperature2m { get; set; }
        public decimal DewPoint2m { get; set; }
        public decimal ApparentTemperature {  get; set; }
        public decimal SurfacePressure { get; set; }
        public decimal Precipitation { get; set; }
        public decimal Rain {  get; set; }
        public decimal Snowfall {  get; set; }
        public int CloudCover { get; set; }
        public int CloudCoverLow { get; set; }
        public decimal Et0FaoEvapotranspiration { get; set; }
        public decimal WindSpeed10m { get; set; }
        public decimal SunshineDuration { get; set; }
        public int WeatherCode { get; set; }
        public string WeatherServiceCode { get; set; } = string.Empty;
        public string OriginalResponse { get; set; } = string.Empty;
    }
}
