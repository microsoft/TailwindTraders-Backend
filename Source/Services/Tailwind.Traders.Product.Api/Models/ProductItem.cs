using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tailwind.Traders.Product.Api.Models
{
    public class ProductItem
    {
        public ProductItem()
        {
            Features = new HashSet<ProductFeature>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public float Price { get; set; }

        public string ImageName { get; set; }

        public int BrandId {get; set;}
        public ProductBrand Brand { get; set; }

        public int TypeId {get; set;}
        public ProductType Type { get; set; }

        public int? TagId { get; set; }
        public ProductTag Tag { get; set; }

        public virtual ICollection<ProductFeature> Features { get; set; }
    }
}
