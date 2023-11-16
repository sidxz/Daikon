using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.EventStore.Settings
{
    public interface IKafkaProducerSettings
    {
        string BootstrapServers { get; }
        string Topic { get; }
    }
}