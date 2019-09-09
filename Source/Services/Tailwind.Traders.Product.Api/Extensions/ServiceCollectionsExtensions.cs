using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Reflection;
using Tailwind.Traders.Product.Api.Infrastructure;
using Tailwind.Traders.Product.Api.Mappers;

namespace Tailwind.Traders.Product.Api.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddProductsContext(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddDbContext<ProductContext>(options =>
            {
                options.UseCosmos(configuration["CosmosDb:Host"], configuration["CosmosDb:Key"], configuration["CosmosDb:Database"])
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            return service;
        }

        public static IServiceCollection AddModulesProducts(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddTransient<IContextSeed<ProductContext>, ProductContextSeed>()
                .AddTransient<IProcessFile, ProccessCsv>()
                .AddTransient<ClassMap, ProductBrandMap>()
                .AddTransient<ClassMap, ProductFeatureMap>()
                .AddTransient<ClassMap, ProductItemMap>()
                .AddTransient<ClassMap, ProductTypeMap>()
                .AddTransient<ClassMap, ProductTagMap>()
                .AddTransient<MapperDtos>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddHttpClient("productVisits", cc =>
                {
                    cc.BaseAddress = new Uri(configuration["ProductVisitsUrl"], UriKind.Absolute);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            service.Configure<AppSettings>(configuration);

            return service;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ConnectionString"],
                    name: "ProductsDB-check",
                    tags: new string[] { "productdb" });
            
            return services;
        }
    }
}
