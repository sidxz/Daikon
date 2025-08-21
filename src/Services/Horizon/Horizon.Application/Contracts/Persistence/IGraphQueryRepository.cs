using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace Horizon.Application.Contracts.Persistence
{
    public interface IGraphQueryRepository
    {
        /* CAUTION: Legacy API.
        * Returning IResultCursor defers session disposal to the caller and can exhaust the Neo4j
        * connection pool under load. Prefer a method that fully materializes results and disposes
        * the session internally (e.g., RunReadAsync). This will be phased out. */
        //public Task<IResultCursor> RunAsync(string query, IDictionary<string, object> parameters);

        /* Use this instead that returns a list of records */
        public Task<IReadOnlyList<IRecord>> RunReadAsync(
        string query,
        object parameters,
        CancellationToken ct = default);

    }
}