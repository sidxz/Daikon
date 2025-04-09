using System.Text.Json;
using Confluent.Kafka;
using Daikon.EventStore.Event;
using Daikon.EventStore.Settings;
using Microsoft.Extensions.Logging;

namespace Daikon.EventStore.Producers
{
    public class EventProducer : IEventProducer, IDisposable
    {
        private readonly ProducerConfig _config;
        private readonly IKafkaProducerSettings _kafkaProducerSettings;
        private readonly ILogger<EventProducer> _logger;
        private readonly int _maxRetries = 5;
        private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(1); // Initial retry delay
        private readonly IProducer<string, string> _producer;

        public EventProducer(IKafkaProducerSettings kafkaProducerSettings, ILogger<EventProducer> logger)
        {
            _kafkaProducerSettings = kafkaProducerSettings ?? throw new ArgumentNullException(nameof(kafkaProducerSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _config = new ProducerConfig
            {
                BootstrapServers = _kafkaProducerSettings.BootstrapServers,
                SecurityProtocol = _kafkaProducerSettings.SecurityProtocol
            };

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

        public async Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent
        {
            int retryCount = 0;
            TimeSpan delay = _initialDelay;

            while (retryCount <= _maxRetries)
            {
                try
                {
                    var eventMessage = new Message<string, string>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = JsonSerializer.Serialize(@event, @event.GetType())
                    };

                    var deliveryResult = await _producer.ProduceAsync(topic, eventMessage);

                    _logger.LogInformation($"Successfully produced {@event.GetType().Name} message to topic '{topic}'.");

                    if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                    {
                        _logger.LogWarning($"Message not persisted: {@event.GetType().Name} to topic '{topic}'.");
                    }

                    break;
                }
                catch (ProduceException<string, string> ex) when (!ex.Error.IsFatal)
                {
                    retryCount++;

                    if (retryCount > _maxRetries)
                    {
                        _logger.LogError(ex, $"Failed to produce {@event.GetType().Name} message to topic '{topic}' after {_maxRetries} retries. Reason: {ex.Error.Reason}");
                        throw new Exception($"Failed to produce {@event.GetType().Name} message to topic '{topic}' after {_maxRetries} retries.", ex);
                    }

                    _logger.LogWarning(ex, $"Retry {retryCount} of {_maxRetries} - Error producing {@event.GetType().Name} to topic '{topic}': {ex.Error.Reason}. Retrying in {delay.TotalSeconds} seconds...");

                    await Task.Delay(delay);
                    delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
                }
                catch (ProduceException<string, string> ex) when (ex.Error.IsFatal)
                {
                    _logger.LogCritical(ex, $"Fatal error producing {@event.GetType().Name} to topic '{topic}': {ex.Error.Reason}");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }
}
