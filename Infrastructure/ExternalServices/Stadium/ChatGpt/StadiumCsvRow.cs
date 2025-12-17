using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Stadium.ChatGpt
{
    public class StadiumCsvRow
    {
        public string TeamName { get; set; } = default!;
        public string TeamFullName { get; set; } = default!;
        public string StadiumOfficialName { get; set; } = default!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
