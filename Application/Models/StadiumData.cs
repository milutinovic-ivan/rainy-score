using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class StadiumData
    {
        public string TeamName { get; set; } = string.Empty;
        public string? TeamFullName { get; set; }
        public string? StadiumOfficialName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
