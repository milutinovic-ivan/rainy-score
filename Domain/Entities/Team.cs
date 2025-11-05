using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? ShortCode { get; set; }
        public int? StadiumId { get; set; }
        public Stadium Stadium { get; set; } = null!;

        [InverseProperty("HomeTeam")]
        public ICollection<MatchDetails> HomeMatches { get; set; } = new List<MatchDetails>();

        [InverseProperty("AwayTeam")]
        public ICollection<MatchDetails> AwayMatches { get; set; } = new List<MatchDetails>();
    }
}
