using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
