using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace Horizon.Infrastructure.Repositories
{
    public class GraphQueryRepository : IGraphQueryRepository
    {
        private readonly IDriver _driver;
        private readonly ILogger<GraphQueryRepository> _logger;

        public GraphQueryRepository(IDriver driver, ILogger<GraphQueryRepository> logger)
        {
            _driver = driver;
            _logger = logger;
        }

        /* CAUTION: Legacy API.
        * Returning IResultCursor defers session disposal to the caller and can exhaust the Neo4j
        * connection pool under load. Prefer a method that fully materializes results and disposes
        * the session internally (e.g., RunReadAsync). This will be phased out. */
        public async Task<IResultCursor> RunAsync(string query, IDictionary<string, object> parameters)
        {
            try
            {
                var session = _driver.AsyncSession();
                var cursor = await session.RunAsync(query, parameters);
                return cursor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running query");
                throw;
            }
        }


        public async Task<IReadOnlyList<IRecord>> RunReadAsync(
        string query,
        object parameters,
        CancellationToken ct = default)
        {
            await using var session = _driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read));
            try
            {
                var cursor = await session.RunAsync(query, parameters);
                var records = await cursor.ToListAsync(ct); // fully consume
                                                            // optional: await cursor.ConsumeAsync();
                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running query");
                throw;
            }
        }
    }
}