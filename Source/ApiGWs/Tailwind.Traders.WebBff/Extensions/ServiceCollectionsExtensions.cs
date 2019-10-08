using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tailwind.Traders.WebBff.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(new Uri($"{ configuration["ProductsApiUrl"]}/liveness"), name: "productapi-check", tags: new string[] { "productapi" })
                .AddUrlGroup(new Uri($"{ configuration["ProfileApiUrl"]}/liveness"), name: "profileapi-check", tags: new string[] { "profileapi" })
                .AddUrlGroup(new Uri($"{ configuration["LoginApiUrl"]}/liveness"), name: "loginapi-check", tags: new string[] { "loginapi" })
                .AddUrlGroup(new Uri($"{ configuration["CouponsApiUrl"]}/liveness"), name: "couponsapi-check", tags: new string[] { "couponsapi" })
                .AddUrlGroup(new Uri($"{ configuration["ImageClassifierApiUrl"]}/liveness"), name: "image-classifier-api-check", tags: new string[] { "imageclassifierapi" })
                .AddUrlGroup(new Uri($"{ configuration["PopularProductsApiUrl"]}/liveness"), name: "popular-products-api-check", tags: new string[] { "popularproductsapi" })
                .AddUrlGroup(new Uri($"{ configuration["StockApiUrl"]}/liveness"), name: "stockapi-check", tags: new string[] { "stockapi" });

            return services;
        }
    }
}