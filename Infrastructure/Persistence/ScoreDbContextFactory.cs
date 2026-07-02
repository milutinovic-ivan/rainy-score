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
            LoadDotEnv();

            //create config from appsettings.json from API project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "API");
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(basePath, "appsettings.json"))
                .Build();
            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? config.GetConnectionString("DefaultConnection");

            // build dbcontext options
            var optionsBuilder = new DbContextOptionsBuilder<ScoreDbContext>();
            optionsBuilder
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();

            // return dbcontext
            return new ScoreDbContext(optionsBuilder.Options);
        }

        private static void LoadDotEnv()
        {
            foreach (var path in GetDotEnvPaths())
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                foreach (var line in File.ReadAllLines(path))
                {
                    var trimmedLine = line.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
                    {
                        continue;
                    }

                    var separatorIndex = trimmedLine.IndexOf('=');

                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    var key = trimmedLine[..separatorIndex].Trim();
                    var value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"');

                    if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
                    {
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }

                return;
            }
        }

        private static IEnumerable<string> GetDotEnvPaths()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            yield return Path.Combine(currentDirectory, ".env");
            yield return Path.Combine(currentDirectory, "..", ".env");
        }
    }
}
