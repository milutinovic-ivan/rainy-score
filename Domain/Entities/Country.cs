namespace Domain.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public ICollection<League> Leagues { get; set; } = new List<League>();
    }
}
