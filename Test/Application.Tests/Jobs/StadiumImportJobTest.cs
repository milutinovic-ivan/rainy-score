using Application.Intefraces;
using Application.Jobs;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;

namespace Tests.Application.Tests.Jobs
{
    public class StadiumImportJobTest : IDisposable
    {
        private readonly SqliteConnection connection;
        private readonly ScoreDbContext dbContext;

        ILogger<StadiumImportJob> _logger;

        IRepository<Stadium> _stadiumRepository;
        IRepository<Team> _teamRepository;

        IUnitOfWork _unitOfWork;

        Mock<IJobExecutionContext> _jobExecutionContext;

        public StadiumImportJobTest()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ScoreDbContext>()
                .UseSqlite(connection)
                .Options;

            dbContext = new ScoreDbContext(options);
            dbContext.Database.EnsureCreated();

            _logger = Mock.Of<ILogger<StadiumImportJob>>();

            _stadiumRepository = new ScoreRepository<Stadium>(dbContext);
            _teamRepository = new ScoreRepository<Team>(dbContext);

            _unitOfWork = new EfUnitOfWork(dbContext);

            _jobExecutionContext = new Mock<IJobExecutionContext>();
        }

        public void Dispose()
        {
            dbContext.Dispose();
            connection.Dispose();
        }

        [Fact]
        public async Task Should_Insert_New_Stadium()
        {
            // ARRANGE
            var stadiumServiceMock = new Mock<IStadiumService>();

            stadiumServiceMock.Setup(s => s.GetStadiumDataAsync(It.IsAny<string>()))
                .ReturnsAsync(new StadiumData
                {
                    Name = "Old Trafford",
                    City = "Manchester",
                    Address = "Sir Matt Busby Way",
                    TerrainType = "grass",
                    Latitude = 40.7128m,
                    Longitude = -74.0060m
                });

            //add a team to the database to link the stadium to
            var team = new Team { Name = "Test Team" };
            await _teamRepository.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            var job = new StadiumImportJob(_stadiumRepository, _teamRepository, stadiumServiceMock.Object, _logger, _unitOfWork);

            // ACT
            await job.Execute(_jobExecutionContext.Object);

            // ASSERT
            var teams = await _teamRepository.GetAllAsync();
            var stadiums = await _stadiumRepository.GetAllAsync();

            Assert.Single(teams);
            Assert.Single(stadiums);

            //test referential integrity
            Assert.Equal(team.StadiumId, stadiums.First().Id);

            //test property mapping
            Assert.Equal("Old Trafford", stadiums.First().Name);
            Assert.Equal("Manchester", stadiums.First().City);
            Assert.Equal("Sir Matt Busby Way", stadiums.First().Address);
            Assert.Equal("grass", stadiums.First().TerrainType);
            Assert.Equal(40.7128m, stadiums.First().Latitude);
            Assert.Equal(-74.0060m, stadiums.First().Longitude);
        }

        [Fact]
        public async Task Should_Not_Insert_Stadium_No_Cordinates()
        {
            // ARRANGE
            var stadiumServiceMock = new Mock<IStadiumService>();

            stadiumServiceMock.Setup(s => s.GetStadiumDataAsync(It.IsAny<string>()))
                .ReturnsAsync(new StadiumData
                {
                    Name = "Old Trafford",
                    City = "Manchester",
                    Address = "Sir Matt Busby Way",
                    TerrainType = "grass",
                    Latitude = null,
                    Longitude = -74.0060m
                });

            //add a team to the database to link the stadium to
            var team = new Team { Name = "Test Team" };
            await _teamRepository.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            var job = new StadiumImportJob(_stadiumRepository, _teamRepository, stadiumServiceMock.Object, _logger, _unitOfWork);

            // ACT
            await job.Execute(_jobExecutionContext.Object);

            // ASSERT
            var teams = await _teamRepository.GetAllAsync();
            var stadiums = await _stadiumRepository.GetAllAsync();
            
            Assert.Single(teams);
            Assert.Empty(stadiums);
        }
    }
}
