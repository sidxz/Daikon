using System.Text.Json;
using Confluent.Kafka;
using Daikon.EventStore.Event;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Logging;

namespace Daikon.EventStore.Producers
{
    /*
     EventProducer is responsible for serializing and publishing domain events to a Kafka topic.
     Includes retry logic with exponential backoff for recoverable errors.
    */
    public class EventProducer : IEventProducer, IDisposable
    {
        private readonly ProducerConfig _config;
        private readonly IKafkaProducerSettings _kafkaProducerSettings;
        private readonly ILogger<EventProducer> _logger;
        private readonly IProducer<string, string> _producer;

        private readonly int _maxRetries = 5;
        private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(1); // Initial delay between retries

        /*
         Constructor initializes Kafka producer with configuration from settings.
        */
        public EventProducer(IKafkaProducerSettings kafkaProducerSettings, ILogger<EventProducer> logger)
        {
            _kafkaProducerSettings = kafkaProducerSettings ?? throw new ArgumentNullException(nameof(kafkaProducerSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _config = new ProducerConfig
            {
                BootstrapServers = _kafkaProducerSettings.BootstrapServers,
                SecurityProtocol = _kafkaProducerSettings.SecurityProtocol
            };

            /* Optionally configure SASL credentials */
            if (!string.IsNullOrEmpty(_kafkaProducerSettings.SaslUsername) &&
                !string.IsNullOrEmpty(_kafkaProducerSettings.SaslPassword))
            {
                _config.SaslMechanism = _kafkaProducerSettings.SaslMechanism;
                _config.SaslUsername = _kafkaProducerSettings.SaslUsername;
                _config.SaslPassword = _kafkaProducerSettings.SaslPassword;
            }

            _producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
        }

        /*
         Produces an event to the specified Kafka topic with retry and exponential backoff.
        */
        public async Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Kafka topic must be specified.", nameof(topic));

            if (@event == null)
                throw new ArgumentNullException(nameof(@event), "Event cannot be null.");

            int retryCount = 0;
            TimeSpan delay = _initialDelay;

            while (retryCount <= _maxRetries)
            {
                try
                {
                    /* Prepare Kafka message */
                    var eventMessage = new Message<string, string>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = JsonSerializer.Serialize(@event, @event.GetType())
                    };

                    /* Send message to Kafka */
                    var deliveryResult = await _producer.ProduceAsync(topic, eventMessage);

                    _logger.LogInformation("✅ Successfully produced {EventType} to topic '{Topic}'.", @event.GetType().Name, topic);

                    if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                    {
                        _logger.LogWarning("⚠️ Kafka message not persisted: {EventType} -> '{Topic}'.", @event.GetType().Name, topic);
                    }

                    break; // Exit loop on success
                }
                catch (ProduceException<string, string> ex) when (!ex.Error.IsFatal)
                {
                    retryCount++;

                    if (retryCount > _maxRetries)
                    {
                        _logger.LogError(ex, "❌ Max retries reached. Failed to produce {EventType} to topic '{Topic}'.", @event.GetType().Name, topic);
                        throw new Exception($"Failed to produce {@event.GetType().Name} to topic '{topic}' after {_maxRetries} retries.", ex);
                    }

                    _logger.LogWarning(
                        ex,
                        "🔁 Retry {RetryCount} of {_maxRetries} - Kafka error producing {EventType} to '{Topic}': {Reason}. Retrying in {DelaySeconds}s...",
                        retryCount,
                        _maxRetries,
                        @event.GetType().Name,
                        topic,
                        ex.Error.Reason,
                        delay.TotalSeconds);

                    await Task.Delay(delay);
                    delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
                }
                catch (ProduceException<string, string> ex) when (ex.Error.IsFatal)
                {
                    _logger.LogCritical(ex, "💥 Fatal Kafka error producing {EventType} to topic '{Topic}': {Reason}", @event.GetType().Name, topic, ex.Error.Reason);
                    throw;
                }
            }
        }

        /*
         Flushes and disposes the Kafka producer.
        */
        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}
