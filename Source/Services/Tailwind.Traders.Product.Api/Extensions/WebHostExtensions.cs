using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                
                seed.SeedAsync(context).Wait();
            }

            return webHost;
        }
    }
}
