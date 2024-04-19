using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Daikon.EventStore.Settings
{
    public class KafkaProducerSettings : IKafkaProducerSettings
    {
        public required string BootstrapServers { get; set; }
        public required string Topic { get; set; }
        public SaslMechanism SaslMechanism { get; set; }
        public SecurityProtocol SecurityProtocol { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }

    }
}