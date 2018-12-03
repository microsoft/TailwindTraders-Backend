using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.Profile.Api.DTOs
{
    public class ProfileDto
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
