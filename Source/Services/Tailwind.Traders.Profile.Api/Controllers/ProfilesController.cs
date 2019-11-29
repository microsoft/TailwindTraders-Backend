using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Infrastructure;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileContext _ctx;
        private readonly AppSettings _settings;

        public ProfileController(ProfileContext ctx, IOptions<AppSettings> options)
        {
            _ctx = ctx;
            _settings = options.Value;
        }

        // GET v1/profile
        [HttpGet]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _ctx.Profiles
                .Select(p => p.ToProfileDto(_settings))
                .ToListAsync();


            if(!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        // GET v1/profile/me
        [HttpGet("me")]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProfile()
        {
            var nameFilter = User.Identity.Name ?? string.Empty;
            var result = await _ctx.Profiles
                            .Where(p => p.Email == nameFilter)
                            .Select(p => p.ToProfileDto(_settings))
                            .SingleOrDefaultAsync();

            if (result == null)
            {
                var defaultUser = GetDefaultUserProfile(nameFilter);
                return Ok(defaultUser);
            }

            return Ok(result);
        }

        // POST v1/profile
        [HttpPost]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // TODO: Auto generated value for int not implemented with CosmosDb EF yet.
            var newId = _ctx.Profiles.ToList().Count();
            var profile = user.MapUserProfile(newId);
            await _ctx.Profiles.AddAsync(profile);
            await _ctx.SaveChangesAsync();

            return Ok();
        }

        private ProfileDto GetDefaultUserProfile(string nameFilter)
        {
            return new ProfileDto
            {
                Id = 0,
                Email = nameFilter,
                Address = "7711 W. Pawnee Ave. Beachwood, OH 44122",
                Name = nameFilter,
                PhoneNumber = "+1-202-555-0155",
                ImageUrlMedium = "defaultImage-m.jpg",
                ImageUrlSmall = "defaultImage-s.jpg"
            };
        }
    }
}
