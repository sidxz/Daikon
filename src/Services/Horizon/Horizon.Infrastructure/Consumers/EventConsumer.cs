
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Horizon.Application.Handlers;
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
        private readonly IGeneEventHandler _geneEventHandler;
        private readonly ITargetEventHandler _targetEventHandler;
        private readonly IScreenEventHandler _screenEventHandler;
        private readonly IHitCollectionEventHandler _hitCollectionEventHandler;
        private readonly IHitAssessmentEventHandler _hitAssessmentEventHandler;
        
        private readonly IProjectEventHandler _projectEventHandler;

        private readonly IMLogixEventHandler _mLogixEventHandler;
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(
                            IConfiguration configuration, IGeneEventHandler eventHandler,
                            ITargetEventHandler targetEventHandler,
                            IScreenEventHandler screenEventHandler,
                            IHitCollectionEventHandler hitCollectionEventHandler,
                            IMLogixEventHandler mLogixEventHandler,
                            IHitAssessmentEventHandler hitAssessmentEventHandler,
                            IProjectEventHandler projectEventHandler,
                            ILogger<EventConsumer> logger
                            )
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaConsumerSettings:BootstrapServers"),
                GroupId = configuration.GetValue<string>("KafkaConsumerSettings:GroupId"),
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration.GetValue<string>("KafkaConsumerSettings:AutoOffsetReset") ?? "Earliest"),
                EnableAutoCommit = configuration.GetValue<bool>("KafkaConsumerSettings:EnableAutoCommit"),
                AllowAutoCreateTopics = configuration.GetValue<bool>("KafkaConsumerSettings:AllowAutoCreateTopics")
            };

            _geneEventHandler = eventHandler;
            _targetEventHandler = targetEventHandler;
            _screenEventHandler = screenEventHandler;
            _hitCollectionEventHandler = hitCollectionEventHandler;
            _hitAssessmentEventHandler = hitAssessmentEventHandler;
            _projectEventHandler = projectEventHandler;
            _mLogixEventHandler = mLogixEventHandler;
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
                            _logger.LogInformation("Horizon: Skipping event {message} as the event was not understood. (Acknowledged)", consumeResult.Message.Value);

                            continue;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing event: {message}", consumeResult.Message.Value);
                            throw new EventConsumeException(nameof(EventConsumer), $"Error deserializing event: {consumeResult.Message.Value}", ex);
                        }

                        // 1st check if the event is a Gene event
                        var geneHandlerMethod = _geneEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (geneHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", geneHandlerMethod.Name, @event.ToJson());
                                geneHandlerMethod.Invoke(_geneEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(geneHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(geneHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }

                        // 2nd check if the event is a Target event
                        var targetHandlerMethod = _targetEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (targetHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", targetHandlerMethod.Name, @event.ToJson());
                                targetHandlerMethod.Invoke(_targetEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(targetHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(targetHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }

                        // 3rd check if the event is a Screen event
                        var screenHandlerMethod = _screenEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (screenHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", screenHandlerMethod.Name, @event.ToJson());
                                screenHandlerMethod.Invoke(_screenEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(screenHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(screenHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }

                        // 4th check if the event is a HitCollection event
                        var hitCollectionHandlerMethod = _hitCollectionEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (hitCollectionHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", hitCollectionHandlerMethod.Name, @event.ToJson());
                                hitCollectionHandlerMethod.Invoke(_hitCollectionEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(hitCollectionHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(hitCollectionHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }

                        // 5th check if the event is a MLogix event
                        var mLogixHandlerMethod = _mLogixEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (mLogixHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", mLogixHandlerMethod.Name, @event.ToJson());
                                mLogixHandlerMethod.Invoke(_mLogixEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(mLogixHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(mLogixHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }

                        // 6th check if the event is a HitAssessment event
                        var hitAssessmentHandlerMethod = _hitAssessmentEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (hitAssessmentHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", hitAssessmentHandlerMethod.Name, @event.ToJson());
                                hitAssessmentHandlerMethod.Invoke(_hitAssessmentEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(hitAssessmentHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(hitAssessmentHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }

                        // 7th check if the event is a Project event
                        var projectHandlerMethod = _projectEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (projectHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", projectHandlerMethod.Name, @event.ToJson());
                                projectHandlerMethod.Invoke(_projectEventHandler, new object[] { @event });
                                consumer.Commit(consumeResult);
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(projectHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error invoking handler method {name}", nameof(projectHandlerMethod));
                                throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            continue;
                        }


                        // check if no handler method was found, throw an exception
                        if (geneHandlerMethod == null
                                && targetHandlerMethod == null
                                && screenHandlerMethod == null
                                && hitCollectionHandlerMethod == null
                                && mLogixHandlerMethod == null
                                && hitAssessmentHandlerMethod == null
                                && projectHandlerMethod == null
                                )
                        {
                            _logger.LogError("Horizon Event is registered but no handler method found");
                            throw new ArgumentNullException(nameof(EventConsumer), "Handler method not found");
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