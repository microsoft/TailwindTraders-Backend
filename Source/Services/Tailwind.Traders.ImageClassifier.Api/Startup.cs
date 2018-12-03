using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Tailwind.Traders.ImageClassifier.Api.Extensions;
using Tailwind.Traders.ImageClassifier.Api.Mlnet;

namespace Tailwind.Traders.ImageClassifier.Api
{
    public class Startup
    {

        private readonly string _contentRoot;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
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
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .AddApplicationInsightsTelemetry(Configuration)
                .AddModulesService(Configuration)
                .AddSecurity();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new QueryStringApiVersionReader();
            });

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Tailwind Traders - Image Classifier API",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
