
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using Daikon.EventStore.Event;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HitAssessment.Application.EventHandlers;
using HitAssessment.Infrastructure.Query.Converters;

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


namespace HitAssessment.Infrastructure.Query.Consumers
{
    public class HitAssessmentEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IHitAssessmentEventHandler _haEventHandler;
        private readonly ILogger<HitAssessmentEventConsumer> _logger;
        public HitAssessmentEventConsumer(IConfiguration configuration, IHitAssessmentEventHandler haEventHandler,
                                    ILogger<HitAssessmentEventConsumer> logger)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaConsumerSettings:BootstrapServers"),

                GroupId = configuration.GetValue<string>("KafkaConsumerSettings:GroupId"),
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration.GetValue<string>("KafkaConsumerSettings:AutoOffsetReset") ?? "Earliest"),
                EnableAutoCommit = configuration.GetValue<bool>("KafkaConsumerSettings:EnableAutoCommit"),
                AllowAutoCreateTopics = configuration.GetValue<bool>("KafkaConsumerSettings:AllowAutoCreateTopics"),

            };

            var securityProtocol = configuration.GetValue<string>("KafkaConsumerSettings:SecurityProtocol");
            if (!string.IsNullOrEmpty(securityProtocol))
            {
                _config.SecurityProtocol = Enum.Parse<SecurityProtocol>(securityProtocol);
            }

            var connectionString = configuration.GetValue<string>("KafkaConsumerSettings:ConnectionString");
            if (!string.IsNullOrEmpty(connectionString))
            {
                _config.SaslMechanism = SaslMechanism.Plain;
                _config.SaslUsername = "$ConnectionString"; 
                _config.SaslPassword = connectionString;
            }

            _haEventHandler = haEventHandler;
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
                            _logger.LogDebug("HitAssessmentEventConsumer: Skipping event {message} as the event was not understood. (Acknowledged)", consumeResult.Message.Value);

                            continue;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing event: {message}", consumeResult.Message.Value);
                            throw new EventConsumeException(nameof(HitAssessmentEventConsumer), $"Error deserializing event: {consumeResult.Message.Value}", ex);
                        }

                        // 1st check if the event is a HitAssessment event
                        var haHandlerMethod = _haEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (haHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", haHandlerMethod.Name, @event.ToJson());
                                haHandlerMethod.Invoke(_haEventHandler, new object[] { @event });
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(haHandlerMethod));
                                throw new EventConsumeException(nameof(HitAssessmentEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            consumer.Commit(consumeResult);
                            continue;
                        }


                        // 3rd check if no handler method was found, throw an exception
                        if (haHandlerMethod == null)
                        {
                            _logger.LogError("Handler method not found {name}", nameof(HitAssessmentEventConsumer));
                            throw new ArgumentNullException(nameof(HitAssessmentEventConsumer), "Handler method not found");
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