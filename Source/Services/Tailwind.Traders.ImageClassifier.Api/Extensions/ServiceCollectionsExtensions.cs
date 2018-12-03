using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tailwind.Traders.ImageClassifier.Api.Security;

namespace Tailwind.Traders.ImageClassifier.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {        
        public static IServiceCollection AddModulesService(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<AppSettings>(configuration);

            return service;
        }

        public static IServiceCollection AddSecurity(this IServiceCollection service)
        {
            service.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            return service;
        }
    }
}
