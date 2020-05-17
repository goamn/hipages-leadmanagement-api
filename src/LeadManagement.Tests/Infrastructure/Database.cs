using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dapper;
using DbUp;
using Npgsql;
using static System.Environment;

namespace Leadmanagement.Tests.Infrastructure
{
    public class Database
    {
        private readonly string _connectionString;
        internal string GetConnectionString => _connectionString;

        public Database(string name)
        {
            var host = GetEnvironmentVariable("DatabaseHost") ?? "localhost";
            var port = GetEnvironmentVariable("DatabasePort") ?? "16201";
            _connectionString = $"Host={host};Port={port};Database={name};Username=postgres;Password='123456'";
        }

        public void Start()
        {
            try
            {
                EnsureDatabase.For.PostgresqlDatabase(_connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create a test database", ex);
            }
        }
    }
}
