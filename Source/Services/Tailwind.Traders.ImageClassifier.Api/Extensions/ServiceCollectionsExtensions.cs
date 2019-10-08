using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tailwind.Traders.ImageClassifier.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {        
        public static IServiceCollection AddModulesService(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<AppSettings>(configuration);

            return service;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            return services;
        }
    }
}
