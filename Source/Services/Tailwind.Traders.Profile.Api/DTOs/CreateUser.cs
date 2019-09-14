using System.ComponentModel.DataAnnotations;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.DTOs
{
    public class CreateUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }

        public Profiles MapUserProfile(int id) =>
            new Profiles()
            {
                Id = id,
                Name = this.Name,
                Address = Address,
                PhoneNumber = PhoneNumber,
                Email = Email
            };
    }    
}
