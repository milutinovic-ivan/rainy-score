namespace Domain.Entities
{
    public class Stadium : BaseEntity
    {
        public string Name { get; set; }
        public string? City { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
