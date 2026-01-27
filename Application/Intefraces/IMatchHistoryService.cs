using Application.Models;

namespace Application.Intefraces
{
    public interface IMatchHistoryService
    {
        Task<List<MatchDetailsData>> GetMatchDetailsHistoriesAsync();
    }
}
