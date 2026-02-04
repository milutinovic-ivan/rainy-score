using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Domain.Entities
{
    public class League : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? ShortCode { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public bool IsCup { get; set; }
        public ICollection<MatchDetails> MatchDetails { get; set; } = new List<MatchDetails>();
        public ICollection<LeagueExternalMap> LeagueExternalMaps { get; set; } = new List<LeagueExternalMap>();
    }
}
