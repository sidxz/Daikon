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
    }
}