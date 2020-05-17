using System;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DbUp;
using Npgsql;
using Serilog;

namespace Leadmanagement.Api.Infrastructure.Database
{
    public class LeadManagementDatabase
    {
        private readonly DatabaseConfiguration _config;

        public LeadManagementDatabase(DatabaseConfiguration config)
        {
            _config = config;
        }

        public async Task<DbConnection> CreateAndOpenConnection(CancellationToken stoppingToken = default)
        {
            var connection = new NpgsqlConnection(_config.ConnectionString);
            await connection.OpenAsync(stoppingToken);

            return connection;
        }

        public void UpgradeIfNecessary()
        {
            Log.Information("Upgrading Database");

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(_config.ConnectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.ToLowerInvariant().Contains("test") == false)
                .LogToAutodetectedLog()
                .WithExecutionTimeout(TimeSpan.FromSeconds(60))
                .Build();

            var result = upgrader.PerformUpgrade();
            if (result.Successful == false)
            {
                Log.Error("Failed to upgrade the database", result.Error);
                throw new Exception();
            }
        }

        public async Task ExecuteInTransaction(Func<DbConnection, DbTransaction, Task> action, CancellationToken cancellationToken = default)
        {
            await using var conn = await CreateAndOpenConnection(cancellationToken);
            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);
            try
            {
                await action(conn, transaction);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Exception while executing transaction - rolling back");
                try
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception ex2)
                {
                    Log.Error(ex2, "Error rolling back transaction");
                }

                throw;
            }
        }

        public void UpgradeWithTestScripts()
        {
            var upgrader = DeployChanges.To
                .PostgresqlDatabase(_config.ConnectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.ToLowerInvariant().Contains("test"))
                .LogToAutodetectedLog()
                .WithExecutionTimeout(TimeSpan.FromSeconds(60))
                .Build();

            var result = upgrader.PerformUpgrade();
            if (result.Successful == false)
            {
                throw new Exception();
            }
        }
    }
}
