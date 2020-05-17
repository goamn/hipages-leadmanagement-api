using Leadmanagement.Api;
using Leadmanagement.Api.Features.Leads;
using Leadmanagement.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Environment;

namespace Leadmanagement.Tests.Infrastructure
{
    public class Server : IAsyncDisposable
    {
        private readonly IWebHost _server;
        private readonly Database _database;
        private readonly ILogger? _logger;

        public HttpClient Client { get; }

        public Server(ILogger? logger = null)
        {
            _logger = logger;
            var resourceSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
            var port = Ports.GetNextAvailablePort();

            _database = new Database(resourceSuffix);
            _server = CreateApiHost(resourceSuffix, port);

            Client = new HttpClient { BaseAddress = new Uri("http://localhost:" + port) };
        }

        public void Start()
        {
            _database.Start();
            _server.Start();
            _server.Services.GetService<LeadManagementDatabase>().UpgradeWithTestScripts();
        }
        public LeadsService GetLeadsService() => _server.Services.GetService<LeadsService>();

        private IWebHost CreateApiHost(string resourceSuffix, int port)
        {
            Program.ConfigureLogging();
            var hostBuilder = Program.CreateHostBuilder(new string[0])
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["ResourceSuffix"] = resourceSuffix,
                        ["DatabaseHost"] = GetEnvironmentVariable("DatabaseHost") ?? "localhost",
                        ["DatabasePort"] = GetEnvironmentVariable("DatabasePort") ?? "16201",
                        ["DatabaseName"] = resourceSuffix,
                    });
                })
                .UseStartup<Startup>()
                .ConfigureKestrel(options => options.ListenAnyIP(port))
                .UseSetting("ResourceSuffix", resourceSuffix)
                .UseEnvironment("Local")
                .ConfigureTestServices(services =>
                {
                    if (_logger != null)
                    {
                        services.AddSingleton(_logger);
                    }
                    services.AddSingleton<Database>();
                });

            var host = hostBuilder.Build();
            return host;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _server.StopAsync(TimeSpan.FromSeconds(5));
                _server.Dispose();
            }
            catch (Exception)
            { }
        }
    }
}