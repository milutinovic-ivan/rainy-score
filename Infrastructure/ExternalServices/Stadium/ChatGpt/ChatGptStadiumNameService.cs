using Application.Intefraces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Stadium.ChatGpt
{
    public class ChatGptStadiumNameService : IStadiumNameService
    {
        private readonly string _folderPath;

        //it will be loaded once per request(scope) and only in requests when this service is in use
        private List<StadiumCsvRow>? _rows;

        private readonly IHostEnvironment _hostEnvironment;

        public ChatGptStadiumNameService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

            _folderPath = Path.GetFullPath(
                    Path.Combine(_hostEnvironment.ContentRootPath, "..", "Infrastructure", "ExternalServices", "Stadium", "ChatGpt", "Files"));
        }

        public (string? teamFullName, string? stadiumOfficialName) GetTeamAndStadiumName(string teamName)
        {
            EnsureLoaded();

            var row = _rows != null ? _rows.FirstOrDefault(r => r.TeamName == teamName) : null;

            return row != null ? (row.TeamFullName, row.StadiumOfficialName) : (null, null);
        }

        private void EnsureLoaded()
        {
            if (_rows != null)
                return;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                HasHeaderRecord = true
            };

            var list = new List<StadiumCsvRow>();

            foreach (var file in Directory.GetFiles(_folderPath, "stadium_batch_*.csv"))
            {
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, config);
                list.AddRange(csv.GetRecords<StadiumCsvRow>());
            }

            _rows = list;
        }
    }
}
