using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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

        public DbSet<LeagueExternalMap> LeagueExternalMaps { get; set; }

        public ScoreDbContext(DbContextOptions<ScoreDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //indexes
            modelBuilder.Entity<Country>().HasIndex(c => c.ShortCode).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();

            //unique but can accept multiple nulls in postgresql
            modelBuilder.Entity<LeagueExternalMap>().HasIndex(l => new { l.ExternalLeagueId, l.DataSource }).IsUnique();
            modelBuilder.Entity<LeagueExternalMap>().HasIndex(l => new { l.LeagueId, l.DataSource }).IsUnique();

            modelBuilder.Entity<MatchDetails>().HasIndex(md => new { md.DataSource, md.FixtureId }).IsUnique();

            //to confirm 1 to 1 relationship in database
            modelBuilder.Entity<WeatherCondition>().HasIndex(w => w.MatchDetailsId).IsUnique();

            modelBuilder.Entity<MatchDetails>()
                .Property(md => md.GoalsOver25Odds)
                .HasColumnName("goals_over_2_5_odds");

            modelBuilder.Entity<MatchDetails>()
                .Property(md => md.GoalsUnder25Odds)
                .HasColumnName("goals_under_2_5_odds");

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
                new League { Id = 1, CountryId = 1, Name = "Premier League", ShortCode = "E0", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 2, CountryId = 1, Name = "Championship", ShortCode = "E1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 3, CountryId = 1, Name = "League One", ShortCode = "E2", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 4, CountryId = 1, Name = "League Two", ShortCode = "E3", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 5, CountryId = 1, Name = "National League", ShortCode = "EC", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 6, CountryId = 2, Name = "Premiership", ShortCode = "SC0", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 7, CountryId = 2, Name = "Championship", ShortCode = "SC1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 8, CountryId = 2, Name = "League One", ShortCode = "SC2", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 9, CountryId = 2, Name = "League Two", ShortCode = "SC3", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 10, CountryId = 3, Name = "Bundesliga", ShortCode = "D1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 11, CountryId = 3, Name = "2. Bundesliga", ShortCode = "D2", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 12, CountryId = 4, Name = "Serie A", ShortCode = "I1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 13, CountryId = 4, Name = "Serie B", ShortCode = "I2", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 14, CountryId = 5, Name = "La Liga", ShortCode = "SP1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 15, CountryId = 5, Name = "Segunda División", ShortCode = "SP2", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 16, CountryId = 6, Name = "Ligue 1", ShortCode = "F1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 17, CountryId = 6, Name = "Ligue 2", ShortCode = "F2", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 18, CountryId = 7, Name = "Eredivisie", ShortCode = "N1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 19, CountryId = 8, Name = "Jupiler Pro League", ShortCode = "B1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 20, CountryId = 9, Name = "Primeira Liga", ShortCode = "P1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 21, CountryId = 10, Name = "Süper Lig", ShortCode = "T1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 22, CountryId = 11, Name = "Super League 1", ShortCode = "G1", IsCup = false, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            modelBuilder.Entity<LeagueExternalMap>().HasData(
                new LeagueExternalMap { Id = 1, LeagueId = 1, ExternalLeagueId = 39, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 2, LeagueId = 2, ExternalLeagueId = 40, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 3, LeagueId = 3, ExternalLeagueId = 41, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 4, LeagueId = 4, ExternalLeagueId = 42, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 5, LeagueId = 5, ExternalLeagueId = 43, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 6, LeagueId = 6, ExternalLeagueId = 179, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 7, LeagueId = 7, ExternalLeagueId = 180, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 8, LeagueId = 8, ExternalLeagueId = 183, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 9, LeagueId = 9, ExternalLeagueId = 184, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 10, LeagueId = 10, ExternalLeagueId = 78, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 11, LeagueId = 11, ExternalLeagueId = 79, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 12, LeagueId = 12, ExternalLeagueId = 135, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 13, LeagueId = 13, ExternalLeagueId = 136, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 14, LeagueId = 14, ExternalLeagueId = 140, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 15, LeagueId = 15, ExternalLeagueId = 141, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 16, LeagueId = 16, ExternalLeagueId = 61, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 17, LeagueId = 17, ExternalLeagueId = 62, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 18, LeagueId = 18, ExternalLeagueId = 88, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 19, LeagueId = 19, ExternalLeagueId = 144, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 20, LeagueId = 20, ExternalLeagueId = 94, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 21, LeagueId = 21, ExternalLeagueId = 203, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new LeagueExternalMap { Id = 22, LeagueId = 22, ExternalLeagueId = 197, DataSource = "apifootball", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            base.OnModelCreating(modelBuilder);
        }
    }   
}
