using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Gene.Application.Query.Handlers;
using Gene.Infrastructure.Query.Converters;
using Microsoft.Extensions.Configuration;

namespace Gene.Infrastructure.Query.Consumers
{
    public class GeneEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IGeneEventHandler _geneEventHandler;
        private readonly IStrainEventHandler _strainEventHandler;
        public GeneEventConsumer(IConfiguration configuration, IGeneEventHandler geneEventHandler, IStrainEventHandler strainEventHandler)
        {
            _config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetValue<string>("KafkaConsumerSettings:BootstrapServers"),
                GroupId = configuration.GetValue<string>("KafkaConsumerSettings:GroupId"),
                AutoOffsetReset = Enum.Parse<AutoOffsetReset>(configuration.GetValue<string>("KafkaConsumerSettings:AutoOffsetReset") ?? "Earliest"),
                EnableAutoCommit = configuration.GetValue<bool>("KafkaConsumerSettings:EnableAutoCommit"),
                AllowAutoCreateTopics = configuration.GetValue<bool>("KafkaConsumerSettings:AllowAutoCreateTopics")
            };

            _geneEventHandler = geneEventHandler;
            _strainEventHandler = strainEventHandler;
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

                // 1st check if the event is a Gene event
                var geneHandlerMethod = _geneEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                if (geneHandlerMethod != null)
                {
                    try
                    {
                        geneHandlerMethod.Invoke(_geneEventHandler, new object[] { @event });
                    }
                    catch (EventHandlerException ex)
                    {
                        throw new EventConsumeException(nameof(GeneEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                    }

                    consumer.Commit(consumeResult);
                    continue;
                }

                // 2nd check if the event is a Strain event
                var strainHandlerMethod = _strainEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                if (strainHandlerMethod != null)
                {
                    try
                    {
                        strainHandlerMethod.Invoke(_strainEventHandler, new object[] { @event });
                    }
                    catch (EventHandlerException ex)
                    {
                        throw new EventConsumeException(nameof(GeneEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                    }

                    consumer.Commit(consumeResult);
                    continue;
                }

                // 3rd check if no handler method was found, throw an exception
                if (geneHandlerMethod == null && strainHandlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(GeneEventConsumer), "Handler method not found");
                }
            }
        }
    }
}