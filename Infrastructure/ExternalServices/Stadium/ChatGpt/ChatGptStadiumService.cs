using Application.Intefraces;
using Application.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Infrastructure.ExternalServices.FootballData;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Stadium.ChatGpt
{
    public class ChatGptStadiumService : IStadiumService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ChatGptStadiumService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public Task<List<StadiumData>> GetStadiumsAsync()
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
                var path = Path.GetFullPath(
                    Path.Combine(_hostEnvironment.ContentRootPath, "..", "Infrastructure", "ExternalServices", "Stadium", "ChatGpt", "Files"));

                var files = Directory.GetFiles(path);

                List<StadiumData> StadiumDataList = new List<StadiumData>();

                int failImportsCount = 0;

                foreach (var file in files)
                {
                    using var reader = new StreamReader(file, Encoding.UTF8);
                    using var csv = new CsvReader(reader, config);

                    csv.Context.RegisterClassMap<ChatGptStadiumMap>();

                    while (csv.Read())
                    {
                        try
                        {
                            var record = csv.GetRecord<StadiumData>();
                            StadiumDataList.Add(record);
                        }
                        catch (TypeConverterException)
                        {
                            failImportsCount++;
                            continue;
                        }
                    }
                }

                Console.WriteLine($"Fail imports count: {failImportsCount}");

                return StadiumDataList;
            });
        }
    }
}
