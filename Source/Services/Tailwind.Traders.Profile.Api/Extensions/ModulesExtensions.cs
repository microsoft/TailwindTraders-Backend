using Microsoft.Extensions.DependencyInjection;
using Tailwind.Traders.Profile.Api.Helpers;
using Tailwind.Traders.Profile.Api.Infrastructure;

namespace Tailwind.Traders.Profile.Api.Extensions
{
    public static class ModulesExtensions
    {
        public static IServiceCollection AddModulesProfile(this IServiceCollection service)
        {
            service.AddScoped<CsvReaderHelper>(); 
            return service;
        }
    }
}
