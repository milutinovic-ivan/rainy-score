using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence
{
    public class ScoreDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<MatchDetails> MatchDetails { get; set; }
        public DbSet<WeatherCondition> WeatherConditions { get; set; }

        public ScoreDbContext(DbContextOptions<ScoreDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //indexes
            modelBuilder.Entity<Country>().HasIndex(c => c.ShortCode).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();

            //unique but can accept multiple nulls
            modelBuilder.Entity<League>().HasIndex(l => l.ApiFootballLeagueId).IsUnique();

            //to confirm 1 to 1 relationship in database
            modelBuilder.Entity<WeatherCondition>().HasIndex(w => w.MatchDetailsId).IsUnique();

            //data seeding
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "England", ShortCode = "ENG", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 2, Name = "Scotland", ShortCode = "SCT", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 3, Name = "Germany", ShortCode = "GER", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 4, Name = "Italy", ShortCode = "ITA", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 5, Name = "Spain", ShortCode = "ESP", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 6, Name = "France", ShortCode = "FRA", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 7, Name = "Netherlands", ShortCode = "NL", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 8, Name = "Belgium", ShortCode = "BEL", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 9, Name = "Portugal", ShortCode = "PRT", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 10, Name = "Turkey", ShortCode = "TUR", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new Country { Id = 11, Name = "Greece", ShortCode = "GRC", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            modelBuilder.Entity<League>().HasData(
                new League { Id = 1, CountryId = 1, Name = "Premier League", ShortCode = "E0", ApiFootballLeagueId = 39, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 2, CountryId = 1, Name = "Championship", ShortCode = "E1", ApiFootballLeagueId = 40, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 3, CountryId = 1, Name = "League One", ShortCode = "E2", ApiFootballLeagueId = 41, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 4, CountryId = 1, Name = "League Two", ShortCode = "E3", ApiFootballLeagueId = 42, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 5, CountryId = 1, Name = "National League", ShortCode = "EC", ApiFootballLeagueId = 43, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 6, CountryId = 2, Name = "Premiership", ShortCode = "SC0", ApiFootballLeagueId = 179, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 7, CountryId = 2, Name = "Championship", ShortCode = "SC1", ApiFootballLeagueId = 180, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 8, CountryId = 2, Name = "League One", ShortCode = "SC2", ApiFootballLeagueId = 183, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 9, CountryId = 2, Name = "League Two", ShortCode = "SC3", ApiFootballLeagueId = 184, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 10, CountryId = 3, Name = "Bundesliga", ShortCode = "D1", ApiFootballLeagueId = 78, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 11, CountryId = 3, Name = "2. Bundesliga", ShortCode = "D2", ApiFootballLeagueId = 79, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 12, CountryId = 4, Name = "Serie A", ShortCode = "I1", ApiFootballLeagueId = 135, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 13, CountryId = 4, Name = "Serie B", ShortCode = "I2", ApiFootballLeagueId = 136, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 14, CountryId = 5, Name = "La Liga", ShortCode = "SP1", ApiFootballLeagueId = 140, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 15, CountryId = 5, Name = "Segunda División", ShortCode = "SP2", ApiFootballLeagueId = 141, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 16, CountryId = 6, Name = "Ligue 1", ShortCode = "F1", ApiFootballLeagueId = 61, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 17, CountryId = 6, Name = "Ligue 2", ShortCode = "F2", ApiFootballLeagueId = 62, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 18, CountryId = 7, Name = "Eredivisie", ShortCode = "N1", ApiFootballLeagueId = 88, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 19, CountryId = 8, Name = "Jupiler Pro League", ShortCode = "B1", ApiFootballLeagueId = 144, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 20, CountryId = 9, Name = "Primeira Liga", ShortCode = "P1", ApiFootballLeagueId = 94, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 21, CountryId = 10, Name = "Süper Lig", ShortCode = "T1", ApiFootballLeagueId = 203, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 22, CountryId = 11, Name = "Super League 1", ShortCode = "G1", ApiFootballLeagueId = 197, IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            base.OnModelCreating(modelBuilder);
        }
    }   
}
