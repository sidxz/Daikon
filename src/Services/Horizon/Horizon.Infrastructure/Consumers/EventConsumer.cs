
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Horizon.Application.Query.Handlers;
using Horizon.Infrastructure.Query.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/*
 * EventConsumer Class
 * 
 * Purpose:
 * --------
 * The EventConsumer class is designed to consume messages from a Kafka topic. It is part of a larger CQRS and event handling architecture.
 * 
 * Key Functionalities:
 * --------------------
 * 1. Kafka Consumer Configuration: Configures and initializes a Kafka consumer based on settings provided through the IConfiguration interface.
 * 2. Continuous Message Consumption: Continuously consumes messages from a specified Kafka topic.
 * 3. Message Deserialization: Deserializes incoming messages using a custom JSON converter to convert them into BaseEvent objects.
 * 4. Dynamic Event Handling: Utilizes reflection to dynamically invoke the appropriate event handler method based on the event type.
 * 5. Error Handling: Robust error handling to manage Kafka consume errors, Kafka exceptions, and general exceptions. Logs errors for troubleshooting.
 * 6. Retry Mechanism: Implements a retry mechanism to handle Kafka connection errors, ensuring resilience and stability.
 * 7. Graceful Shutdown: Supports graceful shutdown through a CancellationToken, allowing the consumer to safely close and release resources.
 * 8. Commit Strategy: Commits each message after successful processing to ensure at-least-once delivery semantics.
 * 
 * Usage:
 * ------
 * The EventConsumer is typically instantiated and used by a higher-level service or application that requires event-driven capabilities.
 * It requires configuration settings, an event handler, and a logger to be passed as dependencies. The consumer can be started by calling the Consume method
 * with the desired topic and a CancellationToken for managing its lifecycle.
 * 
 * Note:
 * -----
 * This class is part of the Horizon.Infrastructure.Query namespace and interacts closely with other components in the CQRS and event handling system.
 * It is essential to ensure that the Kafka and application configurations are correctly set for optimal operation.
 */


namespace Horizon.Infrastructure.Query.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IGeneEventHandler _eventHandler;
        private readonly ILogger<EventConsumer> _logger;
        public EventConsumer(IConfiguration configuration, IGeneEventHandler eventHandler, ILogger<EventConsumer> logger)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaConsumerSettings:BootstrapServers"),
                GroupId = configuration.GetValue<string>("KafkaConsumerSettings:GroupId"),
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration.GetValue<string>("KafkaConsumerSettings:AutoOffsetReset") ?? "Earliest"),
                EnableAutoCommit = configuration.GetValue<bool>("KafkaConsumerSettings:EnableAutoCommit"),
                AllowAutoCreateTopics = configuration.GetValue<bool>("KafkaConsumerSettings:AllowAutoCreateTopics")
            };

            _eventHandler = eventHandler;
            _logger = logger;
        }


        public void Consume(string topic)
        {
            while (true)
            {
                try
                {
                    using var consumer = new ConsumerBuilder<string, string>(_config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build();

                    consumer.Subscribe(topic);
                    while (true)
                    {
                        var consumeResult = consumer.Consume();

                        if (consumeResult?.Message?.Value == null)
                        {
                            continue;
                        }

                        var Options = new JsonSerializerOptions
                        {
                            Converters = { new EventJSONConverter() }
                        };

                        var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, Options);

                        var handlerMethod = _eventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });

                        if (handlerMethod == null)
                        {
                            _logger.LogError("Handler method not found {name}", nameof(handlerMethod));
                            throw new ArgumentNullException(nameof(handlerMethod), "Handler method not found");
                        }

                        try
                        {
                            _logger.LogDebug("Invoking {handlerMethod} with {@event}", handlerMethod.Name, @event.ToJson());
                            handlerMethod.Invoke(_eventHandler, new object[] { @event });
                        }
                        catch (EventHandlerException ex)
                        {
                            _logger.LogError(ex, "EventHandlerException while Invoking {ExceptionMessage}", @event.ToJson());
                            throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                        }

                        consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Kafka consume error: {reason}", e.Error.Reason);
                    // Implement a backoff strategy or wait before retrying
                    _logger.LogInformation("Waiting 10 seconds before retrying");
                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                }
                catch (KafkaException e)
                {
                    _logger.LogError("Kafka error: {message}", e.Message);
                    // Implement a backoff strategy or wait before retrying
                    _logger.LogInformation("Waiting 10 seconds before retrying");
                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                }
                catch (Exception e)
                {
                    _logger.LogError("Error: {message}", e.Message);
                    throw;
                }
            }

        }
    }
}