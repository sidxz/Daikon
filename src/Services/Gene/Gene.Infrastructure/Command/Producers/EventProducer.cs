using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Event;
using CQRS.Core.Producers;
using Microsoft.Extensions.Configuration;

namespace Gene.Infrastructure.Command.Producers
{
    public class EventProducer : IEventProducer
    {
       private readonly ProducerConfig _config;

        public EventProducer(IConfiguration configuration)
        {
            _config = new ProducerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaProducerSettings:BootstrapServers")
            };
        }
        
        public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                                 .SetKeySerializer(Serializers.Utf8)
                                 .SetValueSerializer(Serializers.Utf8)
                                 .Build();
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            };

            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Failed to produce {@event.GetType().Name} message to topic {topic} - {deliveryResult.Message}");
            }
        }
    }
}