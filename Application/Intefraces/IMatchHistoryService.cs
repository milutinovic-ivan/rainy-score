using Application.Models;

namespace Application.Intefraces
{
    public interface IMatchHistoryService
    {
        Task<List<MatchHistoryData>> GetMatchHistoriesAsync();
    }
}
