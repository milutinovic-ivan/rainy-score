using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class StadiumData
    {
        public string Team { get; set; } = string.Empty;
        public string TeamFullName { get; set; } = string.Empty;
        public string Stadium { get; set; }= string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
