namespace API.API.Models.Requests
{
    public class LocationDateRequest
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateOnly Date { get; set; }
    }
}
