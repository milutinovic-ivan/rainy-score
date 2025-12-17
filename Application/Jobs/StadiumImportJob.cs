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
using Application.Models;

namespace Application.Jobs
{
    public class StadiumImportJob : IJob
    {
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IStadiumDataBuilder _stadiumDataBuilder;

        public StadiumImportJob(IRepository<Stadium> stadiumRepository,
            IRepository<Team> teamRepository, IStadiumDataBuilder stadiumDataBuilder)
        {
            _stadiumRepository = stadiumRepository;
            _teamRepository = teamRepository;
            _stadiumDataBuilder = stadiumDataBuilder;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //remove reference to Stadium before delete all Stadiums
            await _teamRepository.GetQuery()
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(t => t.StadiumId, t => null));

            //delete all data from Stadium table
            await _stadiumRepository.DeleteAll();

            var allTeams = await _teamRepository.GetAllAsync();


            var stadiumDataList = new List<StadiumData>();

            //get stadium data from service, one by one
            foreach (Team team in allTeams)
            {
                stadiumDataList.Add(await _stadiumDataBuilder.BuildAsync(team.Name));
            }

            var stadiumToInsert = new List<Stadium>();

            foreach(var stadiumData in stadiumDataList)
            {
                if(stadiumData.StadiumOfficialName == null)
                {
                    continue;
                }

                if(!stadiumToInsert.Exists(s => s.Name == stadiumData.StadiumOfficialName))
                {
                    Stadium stadium = new Stadium()
                    {
                        Name = stadiumData.StadiumOfficialName,
                        Latitude = stadiumData.Latitude,
                        Longitude = stadiumData.Longitude
                    };

                    stadiumToInsert.Add(stadium);
                }
            }

            //insert all at once
            await _stadiumRepository.AddRangeAsync(stadiumToInsert);
            await _stadiumRepository.SaveChangesAsync();

            //update all teams
            var allStadiums = await _stadiumRepository.GetAllAsync();


            try
            {
                foreach (var team in allTeams)
                {
                    var stadiumData = stadiumDataList.Where(sd => sd.TeamName == team.Name).SingleOrDefault();
                    if (stadiumData != null)
                    {
                        var stadium = allStadiums.Where(s => s.Name == stadiumData.StadiumOfficialName).Single();
                        team.StadiumId = stadium.Id;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await _teamRepository.SaveChangesAsync();
        }
    }
}
