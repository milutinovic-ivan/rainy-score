namespace Domain.Entities
{
    public class Stadium : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? TerrainType { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
