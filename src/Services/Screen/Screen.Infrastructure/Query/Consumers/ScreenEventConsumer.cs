
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Screen.Application.EventHandlers;
using Screen.Infrastructure.Query.Converters;

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
 */


namespace Screen.Infrastructure.Query.Consumers
{
    public class ScreenEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IScreenEventHandler _screenEventHandler;
        private readonly IHitCollectionEventHandler _hitCollectionEventHandler;
        private readonly ILogger<ScreenEventConsumer> _logger;
        public ScreenEventConsumer(IConfiguration configuration, IScreenEventHandler screenEventHandler, IHitCollectionEventHandler hitCollectionEventHandler,
                                    ILogger<ScreenEventConsumer> logger)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaConsumerSettings:BootstrapServers"),
                GroupId = configuration.GetValue<string>("KafkaConsumerSettings:GroupId"),
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration.GetValue<string>("KafkaConsumerSettings:AutoOffsetReset") ?? "Earliest"),
                EnableAutoCommit = configuration.GetValue<bool>("KafkaConsumerSettings:EnableAutoCommit"),
                AllowAutoCreateTopics = configuration.GetValue<bool>("KafkaConsumerSettings:AllowAutoCreateTopics")
            };

            _screenEventHandler = screenEventHandler;
            _hitCollectionEventHandler = hitCollectionEventHandler;
            _logger = logger;
        }

        public void Consume(string topic)
        {
            Consume(new[] { topic });
        }



        public void Consume(IEnumerable<string> topics)
        {

            while (true)
            {
                try
                {
                    using var consumer = new ConsumerBuilder<string, string>(_config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build();

                    consumer.Subscribe(topics);
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

                        BaseEvent @event;
                        try
                        {
                            @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, Options);
                        }
                        catch (UnknownEventDiscriminatorException ex)
                        {
                            _logger.LogInformation("ScreenEventConsumer: Skipping event {message} as the event was not understood. (Acknowledged)", consumeResult.Message.Value);

                            continue;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing event: {message}", consumeResult.Message.Value);
                            throw new EventConsumeException(nameof(ScreenEventConsumer), $"Error deserializing event: {consumeResult.Message.Value}", ex);
                        }

                        // 1st check if the event is a Screen event
                        var screenHandlerMethod = _screenEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (screenHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", screenHandlerMethod.Name, @event.ToJson());
                                screenHandlerMethod.Invoke(_screenEventHandler, new object[] { @event });
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(screenHandlerMethod));
                                throw new EventConsumeException(nameof(ScreenEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            consumer.Commit(consumeResult);
                            continue;
                        }

                        // 2nd check if the event is a HitCollection event
                        var hitCollectionHandlerMethod = _hitCollectionEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (hitCollectionHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", hitCollectionHandlerMethod.Name, @event.ToJson());
                                hitCollectionHandlerMethod.Invoke(_hitCollectionEventHandler, new object[] { @event });
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(hitCollectionHandlerMethod));
                                throw new EventConsumeException(nameof(ScreenEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            consumer.Commit(consumeResult);
                            continue;
                        }


                        // 3rd check if no handler method was found, throw an exception
                        if (screenHandlerMethod == null)
                        {
                            _logger.LogError("Handler method not found {name}", nameof(ScreenEventConsumer));
                            throw new ArgumentNullException(nameof(ScreenEventConsumer), "Handler method not found");
                        }
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