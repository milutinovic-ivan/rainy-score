using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Jobs
{
    public class StadiumImportJob : IJob
    {
        private readonly IStadiumService _stadiumService;
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Team> _teamRepository;

        public StadiumImportJob(IStadiumService stadiumService, IRepository<Stadium> stadiumRepository,
            IRepository<Team> teamRepository)
        {
            _stadiumService = stadiumService;
            _stadiumRepository = stadiumRepository;
            _teamRepository = teamRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //remove reference to Stadium before delete all Stadiums
            await _teamRepository.GetQuery()
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(t => t.StadiumId, t => null));

            //delete all data from Stadium table
            await _stadiumRepository.DeleteAll();

            //get stadium data from service
            var stadiumDataList = await _stadiumService.GetStadiumsAsync();

            var stadiumToInsert = new List<Stadium>();

            foreach(var stadiumData in stadiumDataList)
            {
                Stadium stadium = new Stadium()
                {
                    Name = stadiumData.Stadium,
                    City = stadiumData.City,
                    Latitude = stadiumData.Latitude,
                    Longitude = stadiumData.Longitude
                };

                stadiumToInsert.Add(stadium);
            }

            //insert all at once
            await _stadiumRepository.AddRangeAsync(stadiumToInsert);
            await _stadiumRepository.SaveChangesAsync();

            //update all teams
            var allTeams = await _teamRepository.GetAllAsync();
            var allStadiums = await _stadiumRepository.GetAllAsync();


            foreach (var team in allTeams)
            {
                var stadiumData = stadiumDataList.Where(sd => sd.Team == team.Name).SingleOrDefault();
                if (stadiumData != null)
                {
                    var stadium = allStadiums.Where(s => s.Name == stadiumData.Stadium).Single();
                    team.StadiumId = stadium.Id;
                }
            }

            await _teamRepository.SaveChangesAsync();
        }
    }
}
