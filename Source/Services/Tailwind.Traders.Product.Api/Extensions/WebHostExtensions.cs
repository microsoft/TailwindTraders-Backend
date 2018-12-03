using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Tailwind.Traders.Product.Api.Infrastructure;

namespace Tailwind.Traders.Product.Api.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext, TContextSeed>(this IWebHost webHost)
            where TContext : DbContext
            where TContextSeed : IContextSeed<TContext>
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetRequiredService<TContext>();
                var seed = services.GetRequiredService<IContextSeed<TContext>>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database.Migrate();

                    seed.SeedAsync(context).Wait();

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }
}
