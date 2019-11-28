using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using Tailwind.Traders.ImageClassifier.Api.Extensions;
using Tailwind.Traders.ImageClassifier.Api.Mlnet;

namespace Tailwind.Traders.ImageClassifier.Api
{
    public class Startup
    {

        private readonly string _contentRoot;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _contentRoot = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var imgPath = Configuration["ImagePath"];
            if (string.IsNullOrEmpty(imgPath))
            {
                imgPath = Path.Combine(_contentRoot, "_upload");
            }
            else
            {
                if (!Path.IsPathRooted(imgPath))
                {
                    imgPath = Path.Combine(_contentRoot, imgPath);
                }
            }

            if (!Directory.Exists(imgPath))
            {
                Directory.CreateDirectory(imgPath);
            }

            var scoringSvc = new ImageScoringService(_contentRoot, imgPath);
            scoringSvc.Init();

            services
                .AddSingleton<IImageScoringService>(scoringSvc)
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .AddHealthChecks(Configuration)
                .AddApplicationInsightsTelemetry(Configuration)
                .AddModulesService(Configuration);

            var appInsightsIK = Configuration["ApplicationInsights:InstrumentationKey"];

            if (!string.IsNullOrEmpty(appInsightsIK))
            {
                services.AddApplicationInsightsTelemetry(appInsightsIK);
            }

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new QueryStringApiVersionReader();
            });

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Tailwind Traders - Image Classifier API",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image Classifier V1");
                  c.RoutePrefix = string.Empty;
              });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions() { Predicate = r => r.Name.Contains("self") });
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }
}
