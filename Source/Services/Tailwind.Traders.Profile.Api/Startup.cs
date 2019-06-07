using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Tailwind.Traders.Profile.Api.Extensions;

namespace Tailwind.Traders.Profile.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var useB2C = GetUseB2CBoolean();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .AddProfileContext(Configuration)
                .AddModulesProfile();

                //.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //.AddJwtBearer(options =>
                //{
                //    if (useB2C == true)
                //    {
                //        options.Authority = $"https://login.microsoftonline.com/tfp/tailwindtradersB2cTenantdev.onmicrosoft.com/B2C_1_tailwindtraderssigninv1/v2.0/";
                //        options.TokenValidationParameters.ValidateAudience = false;
                //    }
                //    else
                //    {
                //        options.TokenValidationParameters = new TokenValidationParameters
                //        {
                //            ValidateIssuer = true,
                //            ValidateAudience = false,
                //            ValidIssuer = Configuration["Issuer"],
                //            ValidateLifetime = true,
                //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
                //        };
                //    }
                //});

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new QueryStringApiVersionReader();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });

            //app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
        //private bool GetUseB2CBoolean()
        //{
        //    string useB2C = Configuration["UseB2C"];

        //    if (useB2C == null)
        //    {
        //        return false;
        //    }

        //    if (bool.TryParse(useB2C, out bool parsedUseB2C))
        //    {
        //        return parsedUseB2C;
        //    }

        //    return false;
        //}
    }
}
