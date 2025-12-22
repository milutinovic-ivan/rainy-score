using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Intefraces
{
    public interface IWeatherHistoryService
    {
        Task<string?> GetWeatherHistoryResponseAsync(decimal latitude, decimal longitude, DateOnly date);
        WeatherConditionsData PharseWeatherHistoryResponse(string response, TimeOnly time);
    }
}
