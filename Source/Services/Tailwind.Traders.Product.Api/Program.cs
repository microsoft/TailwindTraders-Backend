using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Tailwind.Traders.Product.Api.Extensions;
using Tailwind.Traders.Product.Api.Infrastructure;

namespace Tailwind.Traders.Product.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .MigrateDbContext<ProductContext, ProductContextSeed>()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
