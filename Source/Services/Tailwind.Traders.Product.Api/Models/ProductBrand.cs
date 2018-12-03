using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductBrand
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
