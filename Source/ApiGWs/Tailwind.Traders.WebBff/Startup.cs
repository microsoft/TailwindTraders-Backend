using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using RegistrationUserService;
using System;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Tailwind.Traders.WebBff.Extensions;
using Tailwind.Traders.WebBff.Helpers;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Services;
using static RegistrationUserService.UserServiceClient;

namespace Tailwind.Traders.WebBff
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClientServices();
            services.Configure<AppSettings>(Configuration);
            services.AddTransient<IUserService>(_ => new UserServiceClient(
                EndpointConfiguration.BasicHttpBinding_IUserService,
                new EndpointAddress(Configuration["RegistrationUsersEndpoint"])));

            services.AddTransient<IRegisterService, RegisterService>();

            services.AddSwagger();
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new QueryStringApiVersionReader();
            });

            var appInsightsIK = Configuration["ApplicationInsights:InstrumentationKey"];

            if (!string.IsNullOrEmpty(appInsightsIK))
            {
                services.AddApplicationInsightsTelemetry(appInsightsIK);
            }

            services.AddControllers()
                            .SetCompatibilityVersion(CompatibilityVersion.Latest)
                            .AddNewtonsoftJson()
                            .Services
                            .AddHealthChecks(Configuration)
                            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                if (UseBc2.GetUseB2CBoolean(Configuration))
                                {
                                    options.Authority = Configuration["Authority"];
                                    options.TokenValidationParameters.ValidateAudience = false;
                                    options.RequireHttpsMetadata = false;
                                }
                                else
                                {
                                    options.TokenValidationParameters = new TokenValidationParameters
                                    {
                                        ValidateIssuer = true,
                                        ValidateAudience = false,
                                        ValidIssuer = Configuration["Issuer"],
                                        ValidateLifetime = true,
                                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
                                    };
                                }
                            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var swaggerEndpoint = "/swagger/v1/swagger.json";

            if (!string.IsNullOrEmpty(Configuration["gwPath"]))
            {
                swaggerEndpoint = $"/{Configuration["gwPath"]}{swaggerEndpoint}";
            }

            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerEndpoint, "WebBFF V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions() { Predicate = r => r.Name.Contains("self") });
                endpoints.MapHealthChecks("/readiness", new HealthCheckOptions() { });
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<LPKMessageHandler>();

            //InfinteTimeSpan -> See: https://github.com/aspnet/HttpClientFactory/issues/194
            services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            //add http client services
            services.AddHttpClient(HttpClients.ApiGW)
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                   .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                   .AddHttpMessageHandler<LPKMessageHandler>();

            return services;
        }
    }
}
