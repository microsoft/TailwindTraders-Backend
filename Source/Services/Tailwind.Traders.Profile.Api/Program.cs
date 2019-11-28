using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Tailwind.Traders.Profile.Api.Extensions;
using Tailwind.Traders.Profile.Api.Helpers;
using Tailwind.Traders.Profile.Api.Infrastructure;

namespace Tailwind.Traders.Profile.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .MigrateDbContext<ProfileContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var csvReader = services.GetRequiredService<CsvReaderHelper>();

                    new ProfileContextSeed(csvReader)
                        .SeedAsync(context, env)
                        .Wait();
                })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
