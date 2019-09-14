using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tailwind.Traders.Profile.Api.HealthChecks
{
    public class CosmosDbHealthCheck
        : IHealthCheck
    {
        private static readonly ConcurrentDictionary<string, CosmosClient> _connections = new ConcurrentDictionary<string, CosmosClient>();
        private readonly string _connectionString;
        private readonly string _database;

        public CosmosDbHealthCheck(string connectionString, string database)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _database = database;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                CosmosClient cosmosDbClient;

                if (!_connections.TryGetValue(_connectionString, out cosmosDbClient))
                {
                    cosmosDbClient = new CosmosClient(_connectionString);

                    if (!_connections.TryAdd(_connectionString, cosmosDbClient))
                    {
                        cosmosDbClient.Dispose();
                        cosmosDbClient = _connections[_connectionString];
                    }
                }

                await cosmosDbClient.CreateDatabaseIfNotExistsAsync(_database);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}