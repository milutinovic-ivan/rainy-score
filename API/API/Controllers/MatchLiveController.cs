using Application.Intefraces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchLiveController : ControllerBase
    {
        private readonly IMatchLiveService _matchLiveService;

        public MatchLiveController(IMatchLiveService matchLiveService)
        {
            _matchLiveService = matchLiveService;
        }

        [HttpGet]
        public IActionResult GetMatchLive()
        {
            var matchList = _matchLiveService.GetMatchDetailsListAsync(DateOnly.FromDateTime(DateTime.Today));
            return Ok(matchList);
        }
    }
}
