using Application.Intefraces;
using Application.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Text;

namespace Infrastructure.ExternalServices.MatchHistory.FootballData
{
    public class FootballDataMatchHistoryService : IMatchHistoryService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public FootballDataMatchHistoryService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public Task<List<MatchDetailsData>> GetMatchDetailsHistoriesAsync()
        {
            return Task.Run(() =>
            {
                //set importer configuration
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true
                };

                config.BadDataFound = c =>
                {
                    Console.WriteLine($"Bad data at row {c.Field}: {c.RawRecord}");
                };

                //import all files from directory
                var publishPath = Path.Combine(_hostEnvironment.ContentRootPath, "ExternalServices", "MatchHistory", "FootballData", "Files");

                string path;
                if (Directory.Exists(publishPath))
                {
                    path = publishPath;
                }
                else
                {
                    path = Path.GetFullPath(Path.Combine(_hostEnvironment.ContentRootPath, "..", "Infrastructure", "ExternalServices", "MatchHistory", "FootballData", "Files"));
                }

                var files = Directory.GetFiles(path);

                List<MatchDetailsData> matchHistories = new List<MatchDetailsData>();

                int failImportsCount = 0;

                foreach (var file in files)
                {
                    using var reader = new StreamReader(file, Encoding.UTF8);
                    using var csv = new CsvReader(reader, config);

                    csv.Context.RegisterClassMap<MatchHistoryMap>();

                    while (csv.Read())
                    {
                        try
                        {
                            var matchDetailsData = csv.GetRecord<MatchDetailsData>();
                            matchDetailsData.IsHistory = true;
                            matchHistories.Add(matchDetailsData);
                        }
                        catch (TypeConverterException)
                        {
                            failImportsCount++;
                            continue;
                        }
                    }
                }

                Console.WriteLine($"Fail imports count: {failImportsCount}");

                return matchHistories;
            });
        }
    }
}
