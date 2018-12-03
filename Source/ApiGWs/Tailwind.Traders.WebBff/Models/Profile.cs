using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrlSmall { get; set; }
        public string ImageUrlMedium { get; set; }
    }
}
