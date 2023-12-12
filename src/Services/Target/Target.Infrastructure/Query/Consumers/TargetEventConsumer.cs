
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Target.Application.EventHandlers;
using Target.Infrastructure.Query.Converters;

namespace Target.Infrastructure.Query.Consumers
{
    public class TargetEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly ITargetEventHandler _targetEventHandler;
        private readonly ILogger<TargetEventConsumer> _logger;
        public TargetEventConsumer(IConfiguration configuration, ITargetEventHandler targetEventHandler,
                                    ILogger<TargetEventConsumer> logger)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaConsumerSettings:BootstrapServers"),
                GroupId = configuration.GetValue<string>("KafkaConsumerSettings:GroupId"),
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration.GetValue<string>("KafkaConsumerSettings:AutoOffsetReset") ?? "Earliest"),
                EnableAutoCommit = configuration.GetValue<bool>("KafkaConsumerSettings:EnableAutoCommit"),
                AllowAutoCreateTopics = configuration.GetValue<bool>("KafkaConsumerSettings:AllowAutoCreateTopics")
            };

            _targetEventHandler = targetEventHandler;
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


                        // 1st check if the event is a Target event
                        var targetHandlerMethod = _targetEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (targetHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", targetHandlerMethod.Name, @event.ToJson());
                                targetHandlerMethod.Invoke(_targetEventHandler, new object[] { @event });
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(targetHandlerMethod));
                                throw new EventConsumeException(nameof(TargetEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }
                            consumer.Commit(consumeResult);
                            continue;
                        }

                        // 3rd check if no handler method was found, throw an exception
                        if (targetHandlerMethod == null)
                        {
                            _logger.LogError("Handler method not found {name}", nameof(TargetEventConsumer));
                            throw new ArgumentNullException(nameof(TargetEventConsumer), "Handler method not found");
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