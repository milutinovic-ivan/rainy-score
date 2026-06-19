using Application.Intefraces;
using Application.Jobs;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.ExternalServices.WeatherForecast.OpenMeteo;
using Infrastructure.ExternalServices.WeatherHistory.OpenMeteo;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;

namespace Tests.Application.Tests.Jobs
{
    public class WeatherForecastImportJobTest : IDisposable
    {
        private readonly SqliteConnection connection;
        private readonly ScoreDbContext dbContext;

        ILogger<WeatherForecastImportJob> _logger;

        IRepository<Stadium> _stadiumRepository;
        IRepository<Team> _teamRepository;
        IRepository<MatchDetails> _matchDetailsRepository;
        IRepository<WeatherCondition> _weatherConditionRepository;

        IUnitOfWork _unitOfWork;

        Mock<IJobExecutionContext> _jobExecutionContext;

        public WeatherForecastImportJobTest()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ScoreDbContext>()
                .UseSqlite(connection)
                .Options;

            dbContext = new ScoreDbContext(options);
            dbContext.Database.EnsureCreated();

            _logger = Mock.Of<ILogger<WeatherForecastImportJob>>();

            _stadiumRepository = new ScoreRepository<Stadium>(dbContext);
            _teamRepository = new ScoreRepository<Team>(dbContext);
            _matchDetailsRepository = new ScoreRepository<MatchDetails>(dbContext);
            _weatherConditionRepository = new ScoreRepository<WeatherCondition>(dbContext);

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

            //insert stadium
            var homeStadium = new Stadium
            {
                Name = "Old Trafford",
                City = "Manchester",
                Address = "Sir Matt Busby Way",
                TerrainType = "grass",
                Latitude = 40.7128m,
                Longitude = -74.0060m
            };
            await _stadiumRepository.AddAsync(homeStadium);

            var awayStadium = new Stadium();
            await _stadiumRepository.AddAsync(awayStadium);

            //insert team
            var homeTeam = new Team { Name = "Manchester", CountryId = 1, Stadium = homeStadium };
            await _teamRepository.AddAsync(homeTeam);

            var awayTeam = new Team { Name = "Arsenal", CountryId = 1, Stadium = awayStadium };
            await _teamRepository.AddAsync(awayTeam);

            //insert match details
            await _matchDetailsRepository.AddAsync(new MatchDetails()
            {
                FixtureId = 1001,
                DataSource = "apifootball",

                LeagueId = 1,

                HomeTeam = homeTeam,
                AwayTeam = awayTeam,

                MatchDate = new DateOnly(2026, 6, 19),
                MatchTime = new TimeOnly(21, 00),

                Status = "NS",
                IsHistory = false
            });

            await _unitOfWork.SaveChangesAsync();

            var weatherConditionsData = new WeatherConditionsData
            {
                Temperature2m = 19.3m,
                DewPoint2m = 12.6m,
                Precipitation = 6.4m,
                CloudCover = 95,
                CloudCoverLow = 44,
                WindSpeed10m = 15,
                SunshineDuration = 5,
                WeatherCode = 3,
                WeatherServiceCode = "OpenMeteo",
                OriginalResponse = "original response"
            };

            var weatherForecastServiceMock = new Mock<IWeatherForecastService>();

            weatherForecastServiceMock.Setup(s => s.GetWeatherForecastResponseAsync(It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<DateOnly>()))
                .ReturnsAsync("");
            weatherForecastServiceMock.Setup(s => s.PharseWeatherForecastResponse(It.IsAny<string>(), It.IsAny<TimeOnly>()))
                .Returns(weatherConditionsData);

            var job = new WeatherForecastImportJob(_logger, _matchDetailsRepository, weatherForecastServiceMock.Object, _unitOfWork);

            // ACT
            await job.Execute(_jobExecutionContext.Object);

            // ASSERT
            var weatherConditions = await _weatherConditionRepository.GetAllAsync();
            var weatherCondition = weatherConditions.Single();

            weatherForecastServiceMock.Verify(
                s => s.GetWeatherForecastResponseAsync(40.7128m, -74.0060m, new DateOnly(2026, 6, 19)),
                Times.Once);

            weatherForecastServiceMock.Verify(
                s => s.PharseWeatherForecastResponse("", new TimeOnly(21, 00)),
                Times.Once);

            //test property mapping
            Assert.Equal(weatherConditionsData.Temperature2m, weatherCondition.Temperature2m);
            Assert.Equal(weatherConditionsData.DewPoint2m, weatherCondition.DewPoint2m);
            Assert.Equal(weatherConditionsData.Precipitation, weatherCondition.Precipitation);
            Assert.Equal(weatherConditionsData.CloudCover, weatherCondition.CloudCover);
            Assert.Equal(weatherConditionsData.CloudCoverLow, weatherCondition.CloudCoverLow);
            Assert.Equal(weatherConditionsData.WindSpeed10m, weatherCondition.WindSpeed10m);
            Assert.Equal(weatherConditionsData.SunshineDuration, weatherCondition.SunshineDuration);
            Assert.Equal(weatherConditionsData.WeatherCode, weatherCondition.WeatherCode);
            Assert.Equal(weatherConditionsData.WeatherServiceCode, weatherCondition.WeatherServiceCode);
            Assert.Equal(weatherConditionsData.OriginalResponse, weatherCondition.OriginalResponse);
        }
    }
}
