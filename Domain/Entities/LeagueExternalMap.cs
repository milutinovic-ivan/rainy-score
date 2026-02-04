using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LeagueExternalMap : BaseEntity
    {
        public int LeagueId { get; set; }
        public League League { get; set; } = null!;
        public string DataSource { get; set; } = null!;
        public int ExternalLeagueId { get; set; }
    }
}
