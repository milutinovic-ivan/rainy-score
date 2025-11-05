using Application.Models;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Stadium.ChatGpt
{
    public sealed class ChatGptStadiumMap : ClassMap<StadiumData>
    {
        public ChatGptStadiumMap()
        {
            Map(m => m.Team).Name("Team");
            Map(m => m.TeamFullName).Name("TeamFullName");
            Map(m => m.Stadium).Name("Stadium");
            Map(m => m.City).Name("City");
            Map(m => m.Country).Name("Country");
            Map(m => m.Latitude).Name("Latitude");
            Map(m => m.Longitude).Name("Longitude");
        }
    }
}
