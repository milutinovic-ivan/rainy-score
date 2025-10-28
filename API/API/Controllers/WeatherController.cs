using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models.Requests;
using Application.Intefraces;
using Domain.Entities;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        IWeatherService _weatherService;
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<WeatherData>> GetCurrentForecast([FromQuery]LocationRequest request)
        {
            var data = await _weatherService.GetCurrentWeatherAsync(request.Lat, request.Long);
            return Ok(data);
        }
    }
}
