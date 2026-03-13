using System;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace RepositoryLayer.Redis
{
    public static class RedisConnectionProvider
    {
        private static readonly ConcurrentDictionary<string, Lazy<IConnectionMultiplexer>> connections =
            new ConcurrentDictionary<string, Lazy<IConnectionMultiplexer>>(StringComparer.OrdinalIgnoreCase);

        public static IConnectionMultiplexer ConnectAndPing(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Redis connection string cannot be empty.");
            }

            Lazy<IConnectionMultiplexer> lazy = connections.GetOrAdd(connectionString, cs =>
                new Lazy<IConnectionMultiplexer>(() =>
                {
                    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(cs);
                    _ = multiplexer.GetDatabase().Ping(); // Fail fast (S1)
                    return multiplexer;
                }));

            return lazy.Value;
        }
    }
}