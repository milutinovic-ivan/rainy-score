using Application.Intefraces;
using Domain.Entities;
using Domain.Interfaces;
using Quartz;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Application.Jobs
{
    [DisallowConcurrentExecution]
    public class StadiumImportJob : IJob
    {
        private readonly IRepository<Stadium> _stadiumRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IStadiumService _stadiumService;
        private readonly ILogger<StadiumImportJob> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public StadiumImportJob(IRepository<Stadium> stadiumRepository,
            IRepository<Team> teamRepository,
            IStadiumService stadiumService,
            ILogger<StadiumImportJob> logger,
            IUnitOfWork unitOfWork)
        {
            _stadiumRepository = stadiumRepository;
            _teamRepository = teamRepository;
            _stadiumService = stadiumService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Job started..");

            int stadiumNotFoundCount = 0;
            int stadiumAddedCount = 0;

            //just teams without stadion initialized
            var allTeams = await _teamRepository.GetAllAsync(t => 
                t.Where(t => t.StadiumId == null)
                .Include(t => t.Country));

            foreach (Team team in allTeams)
            {
                var stadiumData = await _stadiumService.GetStadiumDataAsync(team.Name, team.Country!.Name);

                if (stadiumData?.Latitude == null || stadiumData.Longitude == null)
                {
                    _logger.LogWarning($"No coordinates for stadium: {stadiumData?.Name}, team: {team.Name}");
                    stadiumNotFoundCount++;

                    continue;
                }

                Stadium stadium = new Stadium()
                {
                    Name = stadiumData.Name ?? string.Empty,
                    City = stadiumData.City,
                    Address = stadiumData.Address,
                    TerrainType = stadiumData.TerrainType,
                    Latitude = Math.Round(stadiumData.Latitude.Value, 5),
                    Longitude = Math.Round(stadiumData.Longitude.Value, 5)
                };

                //add stadium
                await _stadiumRepository.AddAsync(stadium);

                //add reference to team
                team.Stadium = stadium;
                _teamRepository.Update(team);

                _logger.LogInformation($"Added stadium: {stadium.Name}, latitude: {stadium.Latitude}, longitude: {stadium.Longitude} " +
                    $"for team: {team.Name}");

                stadiumAddedCount++;
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Stadiums added count: {stadiumAddedCount} Stadiums not found count: {stadiumNotFoundCount}");

            _logger.LogInformation("Job finished..");
        }
    }
}
