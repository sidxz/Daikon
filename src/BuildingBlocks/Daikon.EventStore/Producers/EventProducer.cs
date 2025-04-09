using System.Text.Json;
using Confluent.Kafka;
using Daikon.EventStore.Event;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Logging;

namespace Daikon.EventStore.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        private readonly IKafkaProducerSettings _kafkaProducerSettings;
        private readonly ILogger<EventProducer> _logger;
        private readonly int _maxRetries = 5;
        private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(1); // Initial retry delay

        public EventProducer(IKafkaProducerSettings kafkaProducerSettings, ILogger<EventProducer> logger)
        {
            _kafkaProducerSettings = kafkaProducerSettings ?? throw new ArgumentNullException(nameof(kafkaProducerSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Initialize the Kafka Producer configuration
            _config = new ProducerConfig
            {
                BootstrapServers = _kafkaProducerSettings.BootstrapServers,
                SecurityProtocol = _kafkaProducerSettings.SecurityProtocol
            };

            // Apply SASL authentication settings if provided
            if (!string.IsNullOrEmpty(_kafkaProducerSettings.SaslUsername) &&
                !string.IsNullOrEmpty(_kafkaProducerSettings.SaslPassword))
            {
                _config.SaslMechanism = _kafkaProducerSettings.SaslMechanism;
                _config.SaslUsername = _kafkaProducerSettings.SaslUsername;
                _config.SaslPassword = _kafkaProducerSettings.SaslPassword;
            }
        }

        /*
         * Produces an event of type TEvent to the specified Kafka topic.
         * Implements retries with exponential backoff in case of transient failures.
         */
        public async Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent
        {
            int retryCount = 0;
            TimeSpan delay = _initialDelay;

            while (retryCount <= _maxRetries)
            {
                try
                {
                    using var producer = new ProducerBuilder<string, string>(_config)
                        .SetKeySerializer(Serializers.Utf8)
                        .SetValueSerializer(Serializers.Utf8)
                        .Build();

                    // Serialize event data to JSON
                    var eventMessage = new Message<string, string>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = JsonSerializer.Serialize(@event, @event.GetType())
                    };

                    // Produce the message to Kafka
                    var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

                    // Log success
                    _logger.LogInformation($"Successfully produced {@event.GetType().Name} message to topic '{topic}'.");

                    if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                    {
                        _logger.LogWarning($"Message not persisted: {@event.GetType().Name} to topic '{topic}'.");
                    }

                    break; // Exit the loop if the message was produced successfully
                }
                catch (ProduceException<string, string> ex) when (!ex.Error.IsFatal)
                {
                    retryCount++;

                    if (retryCount > _maxRetries)
                    {
                        // Max retry attempts reached, log and rethrow
                        _logger.LogError(ex, $"Failed to produce {@event.GetType().Name} message to topic '{topic}' after {_maxRetries} retries. Reason: {ex.Error.Reason}");
                        throw new Exception($"Failed to produce {@event.GetType().Name} message to topic '{topic}' after {_maxRetries} retries.", ex);
                    }

                    _logger.LogWarning(ex, $"Retry {retryCount} of {_maxRetries} - Error producing {@event.GetType().Name} to topic '{topic}': {ex.Error.Reason}. Retrying in {delay.TotalSeconds} seconds...");

                    await Task.Delay(delay);

                    // Exponential backoff
                    delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
                }
                catch (ProduceException<string, string> ex) when (ex.Error.IsFatal)
                {
                    // Fatal errors should not be retried, log and throw
                    _logger.LogCritical(ex, $"Fatal error producing {@event.GetType().Name} to topic '{topic}': {ex.Error.Reason}");
                    throw;
                }
            }
        }
    }
}
