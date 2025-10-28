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
                new Country { Id = 1, Name = "England", ShortCode = "ENG" }
                );

            modelBuilder.Entity<League>().HasData(
                new League { Id = 1, CountryId = 1, Name = "Premier League", ShortCode = "E0" },
                new League { Id = 2, CountryId = 1, Name = "Championship", ShortCode = "E1" },
                new League { Id = 3, CountryId = 1, Name = "League 1", ShortCode = "E2" },
                new League { Id = 4, CountryId = 1, Name = "League 2", ShortCode = "E3" },
                new League { Id = 5, CountryId = 1, Name = "Conference", ShortCode = "EC" }
                );

            base.OnModelCreating(modelBuilder);
        }
    }   
}
