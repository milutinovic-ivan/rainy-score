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

        public ScoreDbContext(DbContextOptions<ScoreDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "England", ShortCode = "ENG", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            modelBuilder.Entity<League>().HasData(
                new League { Id = 1, CountryId = 1, Name = "Premier League", ShortCode = "E0", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 2, CountryId = 1, Name = "Championship", ShortCode = "E1", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 3, CountryId = 1, Name = "League 1", ShortCode = "E2", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 4, CountryId = 1, Name = "League 2", ShortCode = "E3", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                new League { Id = 5, CountryId = 1, Name = "Conference", ShortCode = "EC", CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            modelBuilder.Entity<Stadium>().HasData(
                new Stadium { Id = 1, Name = "Camp Nou", Latitude = 41.3809m, Longitude = 2.1228m, CreatedAt = new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                );

            base.OnModelCreating(modelBuilder);
        }
    }   
}
