using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Quartz;
using Microsoft.Extensions.Logging;

namespace Application.Jobs
{
    public class StadiumImportJob : IJob
    {
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IStadiumService _stadiumService;
        private readonly ILogger<StadiumImportJob> _logger;

        public StadiumImportJob(IRepository<Stadium> stadiumRepository,
            IRepository<Team> teamRepository,
            IStadiumService stadiumService,
            ILogger<StadiumImportJob> logger)
        {
            _stadiumRepository = stadiumRepository;
            _teamRepository = teamRepository;
            _stadiumService = stadiumService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Job started..");

            var allTeams = await _teamRepository.GetAllAsync();

            foreach (Team team in allTeams)
            {
                //just for teams without stadion initialized
                if(team.StadiumId == null)
                {
                    var stadiumData = await _stadiumService.GetStadiumDataAsync(team.Name);

                    if(stadiumData != null)
                    {
                        Stadium stadium = new Stadium()
                        {
                            Name = stadiumData.Name ?? string.Empty,
                            Latitude = stadiumData.Latitude.HasValue ? Math.Round(stadiumData.Latitude.Value, 5) : null,
                            Longitude = stadiumData.Longitude.HasValue ? Math.Round(stadiumData.Longitude.Value, 5) : null,
                        };

                        //add stadium
                        await _stadiumRepository.AddAsync(stadium);

                        //add reference to team
                        team.Stadium = stadium;
                        _teamRepository.Update(team);

                        _logger.LogInformation($"added stadium: {stadium.Name}, latitude: {stadium.Latitude}, longitude: {stadium.Longitude} " +
                            $"for team: {team.Name}");
                    }
                    else
                    {
                        _logger.LogWarning($"no stadium data for team: {team.Name}");
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(200));
                }
            }

            await _teamRepository.SaveChangesAsync();
            await _stadiumRepository.SaveChangesAsync();

            _logger.LogInformation("Job finished..");
        }
    }
}
