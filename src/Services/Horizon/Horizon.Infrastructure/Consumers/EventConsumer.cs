using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Horizon.Application.Query.Handlers;
using Horizon.Infrastructure.Query.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
                            throw new ArgumentNullException(nameof(handlerMethod), "Handler method not found");
                        }

                        try
                        {
                            handlerMethod.Invoke(_eventHandler, new object[] { @event });
                        }
                        catch (EventHandlerException ex)
                        {
                            throw new EventConsumeException(nameof(EventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                        }

                        consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Kafka consume error: {e.Error.Reason}");
                }
                catch (KafkaException e)
                {
                    _logger.LogError($"Kafka error: {e.Message}");
                    // Implement a backoff strategy or wait before retrying
                    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unexpected error: {e.Message}");
                    throw;
                }
            }

        }
    }
}