using Application.Intefraces;
using Application.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;
using System.Text;

namespace Infrastructure.ExternalServices.FootballData
{
    public class FootballDataMatchHistoryService : IMatchHistoryService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public FootballDataMatchHistoryService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public Task<List<MatchHistoryData>> GetMatchHistoriesAsync()
        {
            return Task.Run(() =>
            {
                var basePath = AppContext.BaseDirectory;
                var path = Path.GetFullPath( 
                    Path.Combine(_hostEnvironment.ContentRootPath, "..", "Infrastructure", "ExternalServices", "FootballData", "Files", "E19", "E0.csv"));

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    TrimOptions = TrimOptions.Trim,
                };

                config.BadDataFound = c =>
                {
                    Console.WriteLine($"Bad data at row {c.Field}: {c.RawRecord}");
                };

                using var reader = new StreamReader(path, Encoding.UTF8);
                using var csv = new CsvReader(reader, config);

                csv.Context.RegisterClassMap<MatchHistoryMap>();

                try
                {
                    var records = csv.GetRecords<MatchHistoryData>().ToList();
                    return records;
                }
                catch (ReaderException ex)
                {
                    Console.WriteLine(ex.InnerException?.Message);
                    throw;
                }                
            });
        }
    }
}
