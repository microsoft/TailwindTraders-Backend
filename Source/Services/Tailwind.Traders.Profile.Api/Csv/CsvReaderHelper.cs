using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Helpers
{
    public class CsvReaderHelper
    {
        private readonly ILogger<CsvReaderHelper> _logger;

        public CsvReaderHelper(ILogger<CsvReaderHelper> logger)
        {
            _logger = logger;
        }

        public IEnumerable<TModel> LoadCsv<TModel>(string root, string fileName)
        {
            try
            {
                _logger.LogDebug($"Start process file from path {Path.Combine(root, "Setup", $"{fileName}.csv")}");

                using (var reader = File.OpenText(Path.Combine(root, "Setup", $"{fileName}.csv")))
                {
                    var _csvReader = new CsvReader(reader);
                    _csvReader.Configuration.RegisterClassMap<ProfilesMap>();

                    var model = _csvReader.GetRecords<TModel>().ToList();

                    _logger.LogDebug($"End process file from path {Path.Combine(root, "Setup", $"{fileName}.csv")}");

                    return model;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Error try to process file from path {Path.Combine(root, "Setup", $"{ fileName}.csv")}");
                throw;
            }
        }
    }    
}
