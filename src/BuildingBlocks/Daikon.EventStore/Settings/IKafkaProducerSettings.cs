using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Daikon.EventStore.Settings
{
    public interface IKafkaProducerSettings 
    {
        string BootstrapServers { get; }
        string Topic { get; }
        SaslMechanism SaslMechanism { get; }
        SecurityProtocol SecurityProtocol { get; }
        string SaslUsername { get; }
        string SaslPassword { get; }

    }
}