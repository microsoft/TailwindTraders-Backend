using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Infrastructure;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Controllers
{
    [Authorize]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileDbContext _ctx;
        private readonly AppSettings _settings;

        public ProfileController(ProfileDbContext ctx, IOptions<AppSettings> options)
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
            var currentUser = HttpContext.User;

            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

            var result = await _ctx.Profiles
                .Where(p => p.Email == userId)
                .Select(p => p.ToProfileDto(_settings))
                .SingleOrDefaultAsync();

            if (result == null)
            {
                var defaultUser = GetDefaultUserProfile(userId);
                return Ok(defaultUser);
            }

            return Ok(result);
        }

        private ProfileDto GetDefaultUserProfile(string userId)
        {
            return new ProfileDto
            {
                Email = userId,
                Address = "7711 W. Pawnee Ave. Beachwood, OH 44122",
                Name = userId,
                PhoneNumber = "+1-202-555-0155",
                Id = 0,
                ImageUrlMedium = "defaultImage-m.jpg",
                ImageUrlSmall = "defaultImage-s.jpg"
            };
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

            await _ctx.Profiles.AddAsync(user.MapUserProfile());
            await _ctx.SaveChangesAsync();

            return Ok();
        }        
    }
}
