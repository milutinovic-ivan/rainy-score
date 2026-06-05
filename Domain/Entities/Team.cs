using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? ShortCode { get; set; }
        public int CountryId { get; set; }
        public Country? Country { get; set; }
        public int? StadiumId { get; set; }
        public Stadium? Stadium { get; set; }

        [InverseProperty("HomeTeam")]
        public ICollection<MatchDetails> HomeMatches { get; set; } = new List<MatchDetails>();

        [InverseProperty("AwayTeam")]
        public ICollection<MatchDetails> AwayMatches { get; set; } = new List<MatchDetails>();
    }
}
