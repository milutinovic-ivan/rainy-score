using Application.Intefraces;
using Application.Jobs;
using Application.Jobs.Services;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;

namespace Tests.Application.Tests.Jobs
{
    public class MatchLiveImportJobTests : IDisposable
    {
        private readonly SqliteConnection connection;
        private readonly ScoreDbContext dbContext;

        ILogger<MatchLiveImportJob> _logger;
        IConfiguration _configuration;

        IRepository<Country> _countryRepository;
        IRepository<League> _leagueRepository;
        IRepository<Team> _teamRepository;
        IRepository<MatchDetails> _matchDetailsRepository;
        IRepository<LeagueExternalMap> _leagueExternalMapRepository;

        IUnitOfWork _unitOfWork;

        IJobExecutionsService _jobExecutionsService;

        public MatchLiveImportJobTests()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ScoreDbContext>()
                .UseSqlite(connection)
                .Options;

            dbContext = new ScoreDbContext(options);
            dbContext.Database.EnsureCreated();

            _logger = Mock.Of<ILogger<MatchLiveImportJob>>();

            _configuration = new ConfigurationBuilder().AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["MatchLiveProviders:ApiFootball:RequestsPerMinute"] = "1000"
                        }).Build();

            _countryRepository = new ScoreRepository<Country>(dbContext);
            _leagueRepository = new ScoreRepository<League>(dbContext);
            _teamRepository = new ScoreRepository<Team>(dbContext);
            _matchDetailsRepository = new ScoreRepository<MatchDetails>(dbContext);
            _leagueExternalMapRepository = new ScoreRepository<LeagueExternalMap>(dbContext);

            _unitOfWork = new EfUnitOfWork(dbContext);

            _jobExecutionsService = new Mock<IJobExecutionsService>().Object;
        }

        public void Dispose()
        {
            dbContext.Dispose();
            connection.Dispose();
        }

        [Fact]
        public async Task Should_Insert_New_Match()
        {
            // Arrange
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 39,

                        Country = "England",
                        LeagueName = "Premier League",

                        HomeTeam = "Liverpool",
                        AwayTeam = "Arsenal",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(18, 30),

                        Status = "NS",
                        IsCup = false
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = 1.90m,
                    DrawWinOdds = 3.50m,
                    AwayWinOdds = 4.20m,

                    GoalsOver25Odds = 1.85m,
                    GoalsUnder25Odds = 1.95m
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // Act
            await job.Execute(context.Object);

            // Assert
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();

            var matchDetails = await _matchDetailsRepository.SingleOrDefaultAsync(m => m.Where(m => m.FixtureId == 1001));

            Assert.Equal(11, countries.Count());
            Assert.Equal(22, leagues.Count());
            Assert.Equal(2, teams.Count());
            Assert.Equal(22, maps.Count());

            Assert.NotNull(matchDetails);
            Assert.Equal(1001, matchDetails.FixtureId);
            Assert.Equal("apifootball", matchDetails.DataSource);
            Assert.Equal(1, matchDetails.LeagueId);

            Assert.Equal(1.90m, matchDetails.HomeWinOdds);
            Assert.Equal(3.50m, matchDetails.DrawWinOdds);
            Assert.Equal(4.20m, matchDetails.AwayWinOdds);

            Assert.Equal(1.85m, matchDetails.GoalsOver25Odds);
            Assert.Equal(1.95m, matchDetails.GoalsUnder25Odds);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Once);
        }

        [Fact]
        public async Task Should_Insert_New_LeagueExternalMap_But_Not_New_Country_And_League()
        {
            // Arrange
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "oddsapi",
                        ExternalLeagueId = 39,

                        Country = "Germany",
                        LeagueName = "Bundesliga",

                        HomeTeam = "Eintracht Frankfurt",
                        AwayTeam = "VfB Stuttgart",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(18, 30),

                        Status = "NS",
                        IsCup = false
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = 1.90m,
                    DrawWinOdds = 3.50m,
                    AwayWinOdds = 4.20m,

                    GoalsOver25Odds = 1.85m,
                    GoalsUnder25Odds = 1.95m
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // Act
            await job.Execute(context.Object);

            // Assert
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();

            Assert.Equal(11, countries.Count());
            Assert.Equal(22, leagues.Count());
            Assert.Equal(2, teams.Count());
            Assert.Equal(23, maps.Count());

            //check league external map
            var map = await _leagueExternalMapRepository.SingleOrDefaultAsync(m => m.Where(m => m.ExternalLeagueId == 39 && m.DataSource == "oddsapi"));

            Assert.NotNull(map);
            Assert.Equal(10, map.LeagueId);

            //check match details
            var matchDetails = await _matchDetailsRepository.SingleOrDefaultAsync(m => m.Where(m => m.FixtureId == 1001));

            Assert.NotNull(matchDetails);
            Assert.Equal(1001, matchDetails.FixtureId);
            Assert.Equal("oddsapi", matchDetails.DataSource);
            Assert.Equal(10, matchDetails.LeagueId);

            Assert.Equal(1.90m, matchDetails.HomeWinOdds);
            Assert.Equal(3.50m, matchDetails.DrawWinOdds);
            Assert.Equal(4.20m, matchDetails.AwayWinOdds);

            Assert.Equal(1.85m, matchDetails.GoalsOver25Odds);
            Assert.Equal(1.95m, matchDetails.GoalsUnder25Odds);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Once);
        }

        [Fact]
        public async Task Should_Insert_New_Country_And_League_And_LeagueExternalMap()
        {
            // ARRANGE
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 200,

                        Country = "Serbia",
                        LeagueName = "Super Liga",

                        HomeTeam = "OFK Beograd",
                        AwayTeam = "Vojvodina",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(18, 30),

                        Status = "NS",
                        IsCup = false
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = 1.90m,
                    DrawWinOdds = 3.50m,
                    AwayWinOdds = 4.20m,

                    GoalsOver25Odds = 1.85m,
                    GoalsUnder25Odds = 1.95m
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // ACT
            await job.Execute(context.Object);

            // ASSERT
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();

            Assert.Equal(12, countries.Count());
            Assert.Equal(23, leagues.Count());
            Assert.Equal(2, teams.Count());
            Assert.Equal(23, maps.Count());

            //check country
            var country = countries.SingleOrDefault(c => c.Name == "Serbia");
            Assert.NotNull(country);

            //check league
            var league = leagues.SingleOrDefault(l => l.Name == "Super Liga" && l.CountryId == country.Id);
            Assert.NotNull(league);


            //check league external map
            var map = maps.SingleOrDefault(m => m.ExternalLeagueId == 200 && m.DataSource == "apifootball");

            Assert.NotNull(map);
            Assert.Equal(league.Id, map.LeagueId);

            //check match details
            var matchDetails = await _matchDetailsRepository.SingleOrDefaultAsync(m => m.Where(m => m.FixtureId == 1001));

            Assert.NotNull(matchDetails);
            Assert.Equal(1001, matchDetails.FixtureId);
            Assert.Equal("apifootball", matchDetails.DataSource);
            Assert.Equal(league.Id, matchDetails.LeagueId);

            Assert.Equal(1.90m, matchDetails.HomeWinOdds);
            Assert.Equal(3.50m, matchDetails.DrawWinOdds);
            Assert.Equal(4.20m, matchDetails.AwayWinOdds);

            Assert.Equal(1.85m, matchDetails.GoalsOver25Odds);
            Assert.Equal(1.95m, matchDetails.GoalsUnder25Odds);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Once);
        }

        [Fact]
        public async Task Should_Insert_Nothing_Because_No_Match_Odds()
        {
            // ARRANGE
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 200,

                        Country = "Serbia",
                        LeagueName = "Super Liga",

                        HomeTeam = "OFK Beograd",
                        AwayTeam = "Vojvodina",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(18, 30),

                        Status = "NS",
                        IsCup = false
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = null,
                    DrawWinOdds = null,
                    AwayWinOdds = null,

                    GoalsOver25Odds = null,
                    GoalsUnder25Odds = null
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // ACT
            await job.Execute(context.Object);

            // ASSERT
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();
            var matchDetails = await _matchDetailsRepository.GetAllAsync();

            //same amount of countries, leagues and maps as data seed
            Assert.Equal(11, countries.Count());
            Assert.Equal(22, leagues.Count());
            Assert.Equal(22, maps.Count());

            //no teams and match details inserted
            Assert.Empty(teams);
            Assert.Empty(matchDetails);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Once);
        }

        [Fact]
        public async Task Should_Insert_Nothing_Because_No_Full_Time_Result()
        {
            // ARRANGE
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 200,

                        Country = "Serbia",
                        LeagueName = "Super Liga",

                        HomeTeam = "OFK Beograd",
                        AwayTeam = "Vojvodina",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(18, 30),

                        Status = "FT",
                        IsCup = false,

                        FullTimeHomeGoals = null,
                        FullTimeAwayGoals = null,
                        HalfTimeHomeGoals = 1,
                        HalfTimeAwayGoals = 0
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = 1.90m,
                    DrawWinOdds = 3.50m,
                    AwayWinOdds = 4.20m,

                    GoalsOver25Odds = 1.85m,
                    GoalsUnder25Odds = 1.95m
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // ACT
            await job.Execute(context.Object);

            // ASSERT
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();
            var matchDetails = await _matchDetailsRepository.GetAllAsync();

            //same amount of countries, leagues and maps as data seed
            Assert.Equal(11, countries.Count());
            Assert.Equal(22, leagues.Count());
            Assert.Equal(22, maps.Count());

            //no teams and match details inserted
            Assert.Empty(teams);
            Assert.Empty(matchDetails);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Never);
        }

        [Fact]
        public async Task Should_Insert_Nothing_Because_It_Is_Cup_Not_League()
        {
            // ARRANGE
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 200,
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                });

            //set that math live service implements league service and that league service returns that league is cup
            var leagueService = matchLiveService.As<ILeagueService>();
            leagueService
                .Setup(x => x.GetLeagueDataAsync(It.IsAny<int>()))
                .ReturnsAsync(new LeagueData
                {
                    Name = "Word cup",
                    IsCup = true
                });


            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // ACT
            await job.Execute(context.Object);

            // ASSERT
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();
            var matchDetails = await _matchDetailsRepository.GetAllAsync();

            //same amount of countries, leagues and maps as data seed
            Assert.Equal(11, countries.Count());
            Assert.Equal(22, leagues.Count());
            Assert.Equal(22, maps.Count());

            //no teams and match details inserted
            Assert.Empty(teams);
            Assert.Empty(matchDetails);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            leagueService.Verify(
                x => x.GetLeagueDataAsync(It.IsAny<int>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Never);
        }

        [Fact]
        public async Task Should_Insert_New_Teams_And_Country_Because_Same_Team_Name_Diff_Country()
        {
            // Arrange
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);

            var team1 = new Team
            {
                Name = "Liverpool",
                CountryId = 1
            };

            var team2 = new Team
            {
                Name = "Arsenal",
                CountryId = 1
            };

            await _teamRepository.AddAsync(team1);
            await _teamRepository.AddAsync(team2);
            await _unitOfWork.SaveChangesAsync();

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 39,

                        //different country
                        Country = "Bulgaria",
                        LeagueName = "Premier League",

                        HomeTeam = "Liverpool",
                        AwayTeam = "Arsenal",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(18, 30),

                        Status = "NS",
                        IsCup = false
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = 1.90m,
                    DrawWinOdds = 3.50m,
                    AwayWinOdds = 4.20m,

                    GoalsOver25Odds = 1.85m,
                    GoalsUnder25Odds = 1.95m
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // Act
            await job.Execute(context.Object);

            // Assert
            var countries = await _countryRepository.GetAllAsync();
            var leagues = await _leagueRepository.GetAllAsync();
            var teams = await _teamRepository.GetAllAsync();
            var maps = await _leagueExternalMapRepository.GetAllAsync();

            var matchDetails = await _matchDetailsRepository.SingleOrDefaultAsync(m => m.Where(m => m.FixtureId == 1001));

            Assert.Equal(12, countries.Count());
            Assert.Equal(22, leagues.Count());
            Assert.Equal(4, teams.Count());
            Assert.Equal(22, maps.Count());

            Assert.NotNull(matchDetails);
            Assert.Equal(1001, matchDetails.FixtureId);
            Assert.Equal("apifootball", matchDetails.DataSource);
            Assert.Equal(1, matchDetails.LeagueId);

            Assert.Equal(1.90m, matchDetails.HomeWinOdds);
            Assert.Equal(3.50m, matchDetails.DrawWinOdds);
            Assert.Equal(4.20m, matchDetails.AwayWinOdds);

            Assert.Equal(1.85m, matchDetails.GoalsOver25Odds);
            Assert.Equal(1.95m, matchDetails.GoalsUnder25Odds);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Once);
        }

        [Fact]
        public async Task Should_Update_Existing_Match_With_Same_Source_And_FixtureId_On_Different_Date()
        {
            // Arrange
            var matchLiveService = new Mock<IMatchLiveService>();

            var runDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var previousDate = runDate.AddDays(-1);

            var homeTeam = new Team
            {
                Name = "Liverpool",
                CountryId = 1
            };

            var awayTeam = new Team
            {
                Name = "Arsenal",
                CountryId = 1
            };

            await _teamRepository.AddAsync(homeTeam);
            await _teamRepository.AddAsync(awayTeam);
            await _unitOfWork.SaveChangesAsync();

            var existingMatch = new MatchDetails
            {
                LeagueId = 1,
                Season = previousDate.Year,
                MatchDate = previousDate,
                MatchTime = new TimeOnly(18, 30),
                HomeTeamId = homeTeam.Id,
                AwayTeamId = awayTeam.Id,
                DataSource = "apifootball",
                FixtureId = 1001,
                Status = "NS",
                IsHistory = false,
                HomeWinOdds = 2.10m,
                DrawWinOdds = 3.20m,
                AwayWinOdds = 3.80m,
                GoalsOver25Odds = 1.90m,
                GoalsUnder25Odds = 1.90m
            };

            await _matchDetailsRepository.AddAsync(existingMatch);
            await _unitOfWork.SaveChangesAsync();

            matchLiveService.Setup(x => x.GetServiceName).Returns("apifootball");

            matchLiveService
                .Setup(x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(new List<MatchDetailsData>
                {
                    new()
                    {
                        FixtureId = 1001,
                        DataSource = "apifootball",
                        ExternalLeagueId = 39,

                        Country = "England",
                        LeagueName = "Premier League",

                        HomeTeam = "Liverpool",
                        AwayTeam = "Arsenal",

                        MatchDate = runDate,
                        MatchTime = new TimeOnly(19, 45),

                        Status = "FT",
                        FullTimeHomeGoals = 2,
                        FullTimeAwayGoals = 1,
                        FullTimeWiner = 'H',
                        HalfTimeHomeGoals = 1,
                        HalfTimeAwayGoals = 1,
                        HalfTimeWiner = 'D',
                        IsCup = false
                    }
                });

            matchLiveService
                .Setup(x => x.GetMatchOddsAsync(1001))
                .ReturnsAsync(new MatchDetailsData
                {
                    BookmakerId = 1,
                    BookmakerName = "Bet365",

                    HomeWinOdds = 1.90m,
                    DrawWinOdds = 3.50m,
                    AwayWinOdds = 4.20m,

                    GoalsOver25Odds = 1.85m,
                    GoalsUnder25Odds = 1.95m
                });

            var job = new MatchLiveImportJob(
                matchLiveService.Object,
                _logger,
                _countryRepository,
                _leagueRepository,
                _teamRepository,
                _matchDetailsRepository,
                _leagueExternalMapRepository,
                _unitOfWork,
                _configuration,
                _jobExecutionsService);

            var context = new Mock<IJobExecutionContext>();

            context.Setup(x => x.CancellationToken)
                .Returns(CancellationToken.None);

            context.Setup(x => x.MergedJobDataMap)
                .Returns(new JobDataMap
                {
                    { "DateOffsetDays", 0 }
                });

            // Act
            await job.Execute(context.Object);

            // Assert
            var matches = await _matchDetailsRepository.GetAllAsync(m => m.Where(m => m.FixtureId == 1001));
            var matchDetails = Assert.Single(matches);

            Assert.Equal(existingMatch.Id, matchDetails.Id);
            Assert.Equal(runDate, matchDetails.MatchDate);
            Assert.Equal(new TimeOnly(19, 45), matchDetails.MatchTime);
            Assert.Equal("FT", matchDetails.Status);
            Assert.Equal(2, matchDetails.FullTimeHomeGoals);
            Assert.Equal(1, matchDetails.FullTimeAwayGoals);
            Assert.Equal(1.90m, matchDetails.HomeWinOdds);
            Assert.Equal(3.50m, matchDetails.DrawWinOdds);
            Assert.Equal(4.20m, matchDetails.AwayWinOdds);

            matchLiveService.Verify(
                x => x.GetMatchDetailsListAsync(It.IsAny<DateOnly>()),
                Times.Once);

            matchLiveService.Verify(
                x => x.GetMatchOddsAsync(1001),
                Times.Once);
        }

    }
}
