using Application.Models;

namespace Application.Intefraces
{
    public interface ILeagueService
    {
        Task<LeagueData?> GetLeagueData(int leagueId);
    }
}
