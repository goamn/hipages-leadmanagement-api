using Leadmanagement.Api.AppStart;
using Leadmanagement.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Microsoft.Extensions.Hosting;
using Leadmanagement.Api.Features.Leads;
using Leadmanagement.Api.Features.Common;

namespace Leadmanagement.Api
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
            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddCustomSwagger();
            services.AddSingleton(Log.Logger);

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddSingleton(DatabaseConfiguration.Create(Configuration));
            services.AddSingleton<LeadManagementDatabase>();

            services.AddSingleton<LeadsService>();
            services.AddSingleton<LeadsRepository>();
            services.AddSingleton<EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices
                .GetRequiredService<LeadManagementDatabase>()
                .UpgradeIfNecessary();

            var basePath = GetBasePath();
            app.UsePathBase(basePath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseCustomSwagger(env.EnvironmentName, basePath);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetBasePath()
        {
            var resourceSuffix = Configuration.GetValue<string>("ResourceSuffix");
            var basePath = $"/{Constants.ApplicationName}{resourceSuffix}";
            return basePath;
        }
    }
}