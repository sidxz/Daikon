using System.Collections.Generic;

namespace SimpleGW.API.Services
{
    public interface IMicroserviceHealthStore
    {
        IReadOnlyDictionary<string, MicroserviceHealthStatus> GetSnapshot();
        void Update(string serviceName, MicroserviceHealthStatus status);
    }
}
