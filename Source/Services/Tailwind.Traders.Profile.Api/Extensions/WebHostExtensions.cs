using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Tailwind.Traders.Profile.Api.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();
                
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                context.Database.Migrate();
                seeder(context, services);

                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }

            return webHost;
        }
    }
}
