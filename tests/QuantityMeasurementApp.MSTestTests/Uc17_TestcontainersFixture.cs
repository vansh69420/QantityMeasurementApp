using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoryLayer.Orm;
using StackExchange.Redis;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace QuantityMeasurementApp.MSTestTests
{
    [TestClass]
    public sealed class Uc17_TestcontainersFixture
    {
        private const string OrmTestDbName = "QuantityMeasurementOrmDb_Test";

        internal static MsSqlContainer SqlContainer { get; private set; } = null!;
        internal static RedisContainer RedisContainer { get; private set; } = null!;

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext _)
        {
            SqlContainer = new MsSqlBuilder()
                .WithPassword("YourStrong!Passw0rd")
                .Build();

            RedisContainer = new RedisBuilder().Build();

            await SqlContainer.StartAsync();
            await RedisContainer.StartAsync();

            Environment.SetEnvironmentVariable("QuantityMeasurement__RepositoryType", "OrmSql");
            Environment.SetEnvironmentVariable("QuantityMeasurement__OrmDatabaseName", OrmTestDbName);

            Environment.SetEnvironmentVariable("Redis__ConnectionString", RedisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ConnectionStrings__QuantityMeasurementDb", SqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("Jwt__SigningKey", "THIS_IS_A_TEST_SIGNING_KEY_CHANGE_ME_1234567890");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "QuantityMeasurementApp");
            Environment.SetEnvironmentVariable("Jwt__Audience", "QuantityMeasurementApp.Client");

            QuantityMeasurementOrmDatabaseInitializer.EnsureMigrated(SqlContainer.GetConnectionString(), OrmTestDbName);

            await ClearRedisAsync();
            await ClearOrmDbAsync();
        }

        [AssemblyCleanup]
        public static async Task AssemblyCleanup()
        {
            await RedisContainer.DisposeAsync();
            await SqlContainer.DisposeAsync();
        }

        internal static async Task ResetStateAsync()
        {
            await ClearRedisAsync();
            await ClearOrmDbAsync();
        }

        private static async Task ClearOrmDbAsync()
        {
            string ormDbConnectionString = QuantityMeasurementOrmConnectionString.BuildOrmConnectionString(
                SqlContainer.GetConnectionString(),
                OrmTestDbName);

            await using SqlConnection connection = new SqlConnection(ormDbConnectionString);
            await connection.OpenAsync();

            // Delete child tables first (triggers will write to AuditLog)
            await using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.RefreshTokens;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            await using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Users;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            await using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.QuantityMeasurementOperations;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Clear audit last so reset state is clean
            await using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.AuditLog;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static async Task ClearRedisAsync()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(RedisContainer.GetConnectionString());
            options.AllowAdmin = true;

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