using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Mappers
{
    public class ProductTagMap : ClassMap<ProductTag>
    {
        public ProductTagMap()
        {
            AutoMap();
        }
    }
}
