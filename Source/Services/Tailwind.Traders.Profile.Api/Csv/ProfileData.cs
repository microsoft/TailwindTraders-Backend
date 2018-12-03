using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.Profile.Api.Csv
{
    public class ProfileData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageNameSmall { get; set; }
        public string ImageNameMedium { get; set; }
    }
}
