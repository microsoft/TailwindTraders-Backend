using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Models
{
    public class Coupons
    {
        public List<SmallCoupon> SmallCoupons { get; set; }
        public BigCoupon BigCoupon { get; set; }
    }

    public class SmallCoupon
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Discount { get; set; }
        public string Until { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }

    public class BigCoupon
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Discount { get; set; }
    }
}
