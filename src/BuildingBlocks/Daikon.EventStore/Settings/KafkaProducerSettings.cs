using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Daikon.EventStore.Settings
{
    public class KafkaProducerSettings : IKafkaProducerSettings
    {
        public required string BootstrapServers { get; set; }
        public required string Topic { get; set; }
    }
}