using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using Gene.Application.Query.Handlers;
using Gene.Infrastructure.Query.Converters;
using Microsoft.Extensions.Configuration;

namespace Gene.Infrastructure.Query.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;

    //     "KafkaConsumerSettings": {
    // "BootstrapServers": "localhost:9092",
    // "Topic": "gene",
    // "GroupId": "gene-consumer-group",
    // "AutoOffsetReset": "Earliest",
    // "EnableAutoCommit": false,
    // "AllowAutoCreateTopics": true
    //
        public EventConsumer(IConfiguration configuration, IEventHandler eventHandler)
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
        }


        public void Consume(string topic)
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

                handlerMethod.Invoke(_eventHandler, new object[] { @event });
                consumer.Commit(consumeResult);
            }
        }
    }
}