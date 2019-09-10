using System.ComponentModel.DataAnnotations.Schema;
using Tailwind.Traders.Profile.Api.DTOs;

namespace Tailwind.Traders.Profile.Api.Models
{
    public class Profiles
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageNameSmall { get; set; }
        public string ImageNameMedium { get; set; }

        public ProfileDto ToProfileDto(AppSettings settings) =>
            new ProfileDto()
            {
                Id = this.Id,
                Name = this.Name,
                Address = this.Address,
                Email = this.Email,
                PhoneNumber = this.PhoneNumber,
                ImageUrlSmall = $"{settings.ProfilesImageUrl}/{this.ImageNameSmall}",
                ImageUrlMedium = $"{settings.ProfilesImageUrl}/{this.ImageNameMedium}"
            };
    }    
}
