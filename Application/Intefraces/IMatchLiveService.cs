using Application.Models;

namespace Application.Intefraces
{
    public interface IMatchLiveService
    {
        Task<List<MatchDetailsData>> GetMatchDetailsListAsync(DateOnly matchDate);
        Task<MatchDetailsData?> GetMatchOddsAsync(int fixtureId);
    }
}
