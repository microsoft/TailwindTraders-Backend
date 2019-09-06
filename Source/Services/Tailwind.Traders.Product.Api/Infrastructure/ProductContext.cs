using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Tailwind.Traders.Product.Api.Models;

namespace Tailwind.Traders.Product.Api.Infrastructure
{
    public class ProductContext : DbContext
    {
        public DbSet<ProductItem> ProductItems { get; set; }

        public DbSet<ProductBrand> ProductBrands { get; set; }

        public DbSet<ProductType> ProductTypes { get; set; }

        public DbSet<ProductFeature> ProductFeatures { get; set; }

        public DbSet<ProductTag> Tags { get; set; }

        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductType>()
                .HasIndex(pt => new { pt.Code })
                .IsUnique();

            base.OnModelCreating(builder);
        }
    }

    public class ProductContextDesignFactory : IDesignTimeDbContextFactory<ProductContext>
    {
        public ProductContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
             .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile($"appsettings.Development.json", optional: true)
             .AddEnvironmentVariables()
             .Build();
            var conn = config.GetValue<string>("ConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<ProductContext>()
                .UseSqlServer(conn);

            return new ProductContext(optionsBuilder.Options);
        }
    }
}
