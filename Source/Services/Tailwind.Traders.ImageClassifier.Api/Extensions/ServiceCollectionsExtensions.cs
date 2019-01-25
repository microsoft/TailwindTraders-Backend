using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tailwind.Traders.ImageClassifier.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {        
        public static IServiceCollection AddModulesService(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<AppSettings>(configuration);

            return service;
        }
    }
}
