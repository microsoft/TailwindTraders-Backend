using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductFeature
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int ProductItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
