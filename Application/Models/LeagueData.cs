namespace Application.Models
{
    public class LeagueData
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool IsCup { get; set; }
    }
}
