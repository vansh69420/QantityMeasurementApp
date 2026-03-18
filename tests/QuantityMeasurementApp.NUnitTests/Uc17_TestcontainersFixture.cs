using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using RepositoryLayer.Orm;
using StackExchange.Redis;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace QuantityMeasurementApp.NUnitTests
{
    [SetUpFixture]
    [NonParallelizable]
    public sealed class Uc17_TestcontainersFixture
    {
        private const string OrmTestDbName = "QuantityMeasurementOrmDb_Test";

        private static readonly MsSqlContainer sqlContainer =
            new MsSqlBuilder()
                .WithPassword("YourStrong!Passw0rd") // SQL Server requirement
                .Build();

        private static readonly RedisContainer redisContainer =
            new RedisBuilder()
                .Build();

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            await sqlContainer.StartAsync();
            await redisContainer.StartAsync();

            // Configure API to run OrmSql + Redis (no in-memory overrides)
            Environment.SetEnvironmentVariable("QuantityMeasurement__RepositoryType", "OrmSql");
            Environment.SetEnvironmentVariable("QuantityMeasurement__OrmDatabaseName", OrmTestDbName);

            Environment.SetEnvironmentVariable("Redis__ConnectionString", redisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb", sqlContainer.GetConnectionString());

            // Ensure ORM test database exists + migrations applied
            QuantityMeasurementOrmDatabaseInitializer.EnsureMigrated(sqlContainer.GetConnectionString(), OrmTestDbName);

            await ClearRedisAsync();
            await ClearOrmDbAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await redisContainer.DisposeAsync();
            await sqlContainer.DisposeAsync();
        }

        public static async Task ResetStateAsync()
        {
            await ClearRedisAsync();
            await ClearOrmDbAsync();
        }

        private static async Task ClearOrmDbAsync()
        {
            // Cleanup B1: clear Operations + AuditLog
            string ormDbConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                sqlContainer.GetConnectionString(),
                OrmTestDbName);

            await using SqlConnection connection = new SqlConnection(ormDbConnectionString);
            await connection.OpenAsync();

            // Order: Audit first is ok; both are independent
            await using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.AuditLog;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            await using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.QuantityMeasurementOperations;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static async Task ClearRedisAsync()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(redisContainer.GetConnectionString());            options.AllowAdmin = true;

            IConnectionMultiplexer multiplexer = await ConnectionMultiplexer.ConnectAsync(options);

            EndPoint[] endpoints = multiplexer.GetEndPoints();
            foreach (EndPoint endpoint in endpoints)
            {
                IServer server = multiplexer.GetServer(endpoint);
                await server.FlushDatabaseAsync();
            }

            await multiplexer.CloseAsync();
        }
    }
}