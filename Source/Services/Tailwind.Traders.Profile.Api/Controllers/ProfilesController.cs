using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Tailwind.Traders.Profile.Api.DTOs;
using Tailwind.Traders.Profile.Api.Infrastructure;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Controllers
{
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
        [Authorize]
        public async Task<IActionResult> GetAllProfiles()
        {
            var result = await _ctx.Profiles
                .Select(p => p.ToProfileDto(_settings))
                .ToListAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        // GET v1/profile/me
        [HttpGet("me")]
        [ProducesResponseType(typeof(List<Profiles>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            StringValues headerValues;
            var nameFilter = string.Empty;

            if (Request.Headers.TryGetValue("x-tt-name", out headerValues))
            {
                nameFilter = User.Identity.Name;
            }

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

        private ProfileDto GetDefaultUserProfile(string nameFilter)
        {
            return new ProfileDto
            {
                Email = $"{nameFilter}@{nameFilter}.com",
                Address = "7711 W. Pawnee Ave. Beachwood, OH 44122",
                Name = nameFilter,
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
        [Authorize]
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
