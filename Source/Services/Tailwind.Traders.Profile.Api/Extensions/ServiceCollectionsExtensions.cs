using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Reflection;
using Tailwind.Traders.Profile.Api.Infrastructure;

namespace Tailwind.Traders.Profile.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddProfileContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<AppSettings>(configuration);

            service.AddDbContext<ProfileDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"], sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(45), errorNumbersToAdd: null);
                })
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            },
                ServiceLifetime.Scoped
            );

            return service;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ConnectionString"],
                    name: "ProfileDB-check",
                    tags: new string[] { "profiledb" });

            return services;
        }
    }
}
