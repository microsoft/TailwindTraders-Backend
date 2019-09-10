using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tailwind.Traders.Profile.Api.HealthChecks;
using Tailwind.Traders.Profile.Api.Infrastructure;

namespace Tailwind.Traders.Profile.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddProfileContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<AppSettings>(configuration);
            service.AddDbContext<ProfileContext>(options =>
            {
                options.UseCosmos(configuration["CosmosDb:Host"], configuration["CosmosDb:Key"], configuration["CosmosDb:Database"])
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);

            return service;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder.Add(new HealthCheckRegistration(
                "ProfileDB-check",
                sp => new CosmosDbHealthCheck(
                    $"AccountEndpoint={configuration["CosmosDb:Host"]};AccountKey={configuration["CosmosDb:Key"]}",
                    configuration["CosmosDb:Database"]),
                HealthStatus.Unhealthy,
                new string[] { "profileDb"}
            ));

            return services;
        }
    }
}
