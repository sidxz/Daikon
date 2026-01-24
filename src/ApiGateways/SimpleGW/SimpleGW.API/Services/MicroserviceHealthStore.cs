using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SimpleGW.API.Services
{
    public sealed class MicroserviceHealthStore : IMicroserviceHealthStore
    {
        private readonly ConcurrentDictionary<string, MicroserviceHealthStatus> _statuses =
            new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, MicroserviceHealthStatus> GetSnapshot()
        {
            return new Dictionary<string, MicroserviceHealthStatus>(_statuses, StringComparer.OrdinalIgnoreCase);
        }

        public void Update(string serviceName, MicroserviceHealthStatus status)
        {
            _statuses[serviceName] = status;
        }
    }
}
