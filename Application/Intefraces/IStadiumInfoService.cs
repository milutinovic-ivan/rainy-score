using Application.Models;

namespace Application.Intefraces
{
    public interface IStadiumInfoService
    {
        Task<StadiumInfo?> GetStadiumInfoAsync(string teamName);
    }
}
