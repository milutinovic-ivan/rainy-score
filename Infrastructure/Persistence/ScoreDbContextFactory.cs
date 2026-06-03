using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public class ScoreDbContextFactory : IDesignTimeDbContextFactory<ScoreDbContext>
    {
        public ScoreDbContext CreateDbContext(string[] args)
        {
            //create config from appsettings.json from API project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "API");
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(basePath, "appsettings.json"))
                .Build();

            // build dbcontext options
            var optionsBuilder = new DbContextOptionsBuilder<ScoreDbContext>();
            optionsBuilder
                .UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();

            // return dbcontext
            return new ScoreDbContext(optionsBuilder.Options);
        }
    }
}
