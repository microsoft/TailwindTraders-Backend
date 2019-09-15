using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Tailwind.Traders.Profile.Api.Models;

namespace Tailwind.Traders.Profile.Api.Infrastructure
{
    public class ProfileDbContext : DbContext
    {
        public DbSet<Profiles> Profiles { get; set; }

        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profiles>()
                .HasAlternateKey(c => c.Email);
        }        
    }
}
