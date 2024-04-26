
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Event;
using CQRS.Core.Producers;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Logging;

/*
== Overview ==
The EventProducer class, part of the Daikon.EventStore.Producers namespace, implements the IEventProducer interface and is responsible 
for producing events to a Kafka topic. 
It is designed to work within systems using the CQRS (Command Query Responsibility Segregation) and Event Sourcing patterns, 
specifically for event production in a Kafka-based messaging environment.

*/
namespace Daikon.EventStore.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        private readonly IKafkaProducerSettings _kafkaProducerSettings;
        private readonly ILogger<EventProducer> _logger;

        private readonly int maxRetries = 5;

        public EventProducer(IKafkaProducerSettings kafkaProducerSettings, ILogger<EventProducer> logger)
        {
            _kafkaProducerSettings = kafkaProducerSettings;
            _config = new ProducerConfig
            {
                BootstrapServers = _kafkaProducerSettings.BootstrapServers,
                SecurityProtocol = _kafkaProducerSettings.SecurityProtocol,
            };

            // Check if SASL settings are provided and apply them
            if (!string.IsNullOrEmpty(_kafkaProducerSettings.SaslUsername) &&
                !string.IsNullOrEmpty(_kafkaProducerSettings.SaslPassword))
            {
                _config.SaslMechanism = _kafkaProducerSettings.SaslMechanism;
                _config.SaslUsername = _kafkaProducerSettings.SaslUsername;
                _config.SaslPassword = _kafkaProducerSettings.SaslPassword;
            }

            _logger = logger;
        }

        /*
         ProduceAsync<TEvent>(string topic, TEvent @event):
            Asynchronously produces an event of type TEvent (where TEvent is a subclass of BaseEvent) to the specified Kafka topic.
            Utilizes System.Text.Json.JsonSerializer for serializing the event object into a JSON string.
            Generates a unique key for each message using Guid.NewGuid().
            Throws an exception if the message is not persisted successfully in Kafka.
        */
        public async Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent
        {
            int retryCount = 0;
            TimeSpan delay = TimeSpan.FromSeconds(1); // Initial delay, will increase exponentially

            while (true)
            {
                try
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
                    _logger.LogInformation($"Produced {@event.GetType().Name} message to topic {topic} - {deliveryResult.Message}");
                    _logger.LogDebug($"DEBUG: {@event.GetType().Name} eventMessage.Value" + eventMessage.Value);

                    if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                    {
                        _logger.LogWarning($"[Retry {retryCount} of {maxRetries}] Failed to produce {@event.GetType().Name} message to topic {topic} - {deliveryResult.Message}");
                    }

                    break;
                }
                catch (ProduceException<string, string> ex) when (ex.Error.IsFatal == false)
                {
                    if (retryCount++ == maxRetries)
                    {
                        _logger.LogError($"Failed to produce {@event.GetType().Name} message to topic {topic} - {ex.Error.Reason}");
                        throw new Exception($"Failed to produce {@event.GetType().Name} message to topic {topic} - {ex.Error.Reason}");
                    }
                    else
                    {
                        await Task.Delay(delay);
                        // Increase the delay for the next retry (exponential backoff)
                        delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
                    }
                }

            }

        }
    }
}