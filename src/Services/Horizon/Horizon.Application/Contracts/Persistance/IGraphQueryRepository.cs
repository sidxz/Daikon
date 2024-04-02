using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphQueryRepository
    {
        public Task<IResultCursor> RunAsync(string query, IDictionary<string, object> parameters);
    }
}