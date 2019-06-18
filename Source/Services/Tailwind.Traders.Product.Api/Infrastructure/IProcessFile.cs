using CsvHelper.Configuration;
using System.Collections.Generic;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public interface IProcessFile
    {
        IEnumerable<TModel> Process<TModel>(string root, string fileName, Configuration cfg = null);
    }
}
