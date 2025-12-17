using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models.Requests;
using Application.Intefraces;
using Domain.Entities;
using System.Threading.Tasks;
using Application.Models;
using API.API.Models.Requests;

namespace API.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IWeatherHistoryService _weatherHistoryService;

        public WeatherController(IWeatherService weatherService, IWeatherHistoryService weatherHistoryService)
        {
            _weatherService = weatherService;
            _weatherHistoryService = weatherHistoryService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<WeatherData>> GetCurrentForecast([FromQuery]LocationRequest request)
        {
            var data = await _weatherService.GetCurrentWeatherAsync(request.Lat, request.Long);
            return Ok(data);
        }

        [HttpGet("history")]
        public async Task<ActionResult<WeatherConditionsData>> GetHistoryConditions([FromQuery]LocationDateRequest request)
        {
            var data = await _weatherHistoryService.GetWeatherHistoryResponseAsync(request.Latitude, request.Longitude, request.Date);
            var response = _weatherHistoryService.PharseWeatherHistoryResponse(data, new TimeOnly(19, 15));
            return Ok(response);
        }
    }
}
