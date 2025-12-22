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
                new League { Id = 1, CountryId = 1, Name = "Premier League", ShortCode = "E0", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 2, CountryId = 1, Name = "Championship", ShortCode = "E1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 3, CountryId = 1, Name = "League 1", ShortCode = "E2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 4, CountryId = 1, Name = "League 2", ShortCode = "E3", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 5, CountryId = 1, Name = "Conference", ShortCode = "EC", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 6, CountryId = 2, Name = "Premier League", ShortCode = "SC0", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 7, CountryId = 2, Name = "Division 1", ShortCode = "SC1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 8, CountryId = 2, Name = "Division 2", ShortCode = "SC2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 9, CountryId = 2, Name = "Division 3", ShortCode = "SC3", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 10, CountryId = 3, Name = "Bundesliga 1", ShortCode = "D1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 11, CountryId = 3, Name = "Bundesliga 2", ShortCode = "D2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 12, CountryId = 4, Name = "Serie A", ShortCode = "I1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 13, CountryId = 4, Name = "Serie B", ShortCode = "I2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 14, CountryId = 5, Name = "La Liga Primera Division", ShortCode = "SP1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 15, CountryId = 5, Name = "La Liga Segunda Division", ShortCode = "SP2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 16, CountryId = 6, Name = "Le Championnat", ShortCode = "F1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 17, CountryId = 6, Name = "Division 2", ShortCode = "F2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 18, CountryId = 7, Name = "Eredivisie", ShortCode = "N1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 19, CountryId = 8, Name = "Jupiler League", ShortCode = "B1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 20, CountryId = 9, Name = "Liga I", ShortCode = "P1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 21, CountryId = 10, Name = "Futbol Ligi 1", ShortCode = "T1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },

                new League { Id = 22, CountryId = 11, Name = "Ethniki Katigoria", ShortCode = "G1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            base.OnModelCreating(modelBuilder);
        }
    }   
}
