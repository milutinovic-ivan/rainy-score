using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace Domain.Entities
{
    public class League : BaseEntity
    {
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public ICollection<MatchDetails> MatchDetails { get; set; } = new List<MatchDetails>();
    }
}
