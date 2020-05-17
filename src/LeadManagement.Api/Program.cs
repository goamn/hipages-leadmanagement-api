using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Leadmanagement.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ConfigureLogging();

            try
            {
                Log.Information("Starting host");
                CreateHostBuilder(args)
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", Constants.ApplicationName)
                .Enrich.WithProperty("System", Constants.SystemName)
                .Enrich.WithProperty("ResourceSuffix", Environment.GetEnvironmentVariable("ResourceSuffix"))
                .Enrich.WithProperty("Version", typeof(Program).Assembly.GetName().Version?.ToString())
                .WriteTo.Console(new JsonFormatter())
                .CreateLogger();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog();
    }
}

