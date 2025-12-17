using Application.Intefraces;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Builders
{
    public class StadiumDataBuilder : IStadiumDataBuilder
    {
        private readonly IStadiumNameService _stadiumNameService;
        private readonly IStadiumLocationService _stadiumLocationService;

        public StadiumDataBuilder(IStadiumNameService stadiumNameService, IStadiumLocationService stadiumLocationService)
        {
            _stadiumNameService = stadiumNameService;
            _stadiumLocationService = stadiumLocationService;
        }

        public async Task<StadiumData> BuildAsync(string teamName)
        {
            var (teamFullName, stadiumOfficialName) = _stadiumNameService.GetTeamAndStadiumName(teamName);

            decimal? latitude = null;
            decimal? longitude = null;

            if (stadiumOfficialName != null)
            {
                (latitude, longitude) = await _stadiumLocationService.GetStadiumLocationAsync(stadiumOfficialName);
            }

            var stadiumData = new StadiumData
            {
                TeamName = teamName,
                TeamFullName = teamFullName,
                StadiumOfficialName = stadiumOfficialName,
                Latitude = latitude,
                Longitude = longitude
            };

            return stadiumData;
        }
    }
}
