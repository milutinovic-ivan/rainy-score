using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class StadiumData
    {
        public string Name { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
