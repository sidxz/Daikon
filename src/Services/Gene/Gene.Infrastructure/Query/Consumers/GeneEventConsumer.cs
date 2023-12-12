
using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using Gene.Application.Query.EventHandlers;
using Gene.Infrastructure.Query.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gene.Infrastructure.Query.Consumers
{
    public class GeneEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IGeneEventHandler _geneEventHandler;
        private readonly IStrainEventHandler _strainEventHandler;
        private readonly ILogger<GeneEventConsumer> _logger;
        public GeneEventConsumer(IConfiguration configuration, IGeneEventHandler geneEventHandler, IStrainEventHandler strainEventHandler, ILogger<GeneEventConsumer> logger)
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
                        

                        // 1st check if the event is a Gene event
                        var geneHandlerMethod = _geneEventHandler.GetType().GetMethod("OnEvent", new Type[] { @event.GetType() });
                        if (geneHandlerMethod != null)
                        {
                            try
                            {
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", geneHandlerMethod.Name, @event.ToJson());
                                geneHandlerMethod.Invoke(_geneEventHandler, new object[] { @event });
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(geneHandlerMethod));
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
                                _logger.LogDebug("Invoking {handlerMethod} with {@event}", strainHandlerMethod.Name, @event.ToJson());
                                strainHandlerMethod.Invoke(_strainEventHandler, new object[] { @event });
                            }
                            catch (EventHandlerException ex)
                            {
                                _logger.LogError("Handler method not found {name}", nameof(geneHandlerMethod));
                                throw new EventConsumeException(nameof(GeneEventConsumer), $"Error Invoking {@event.ToJson()}", ex);
                            }

                            consumer.Commit(consumeResult);
                            continue;
                        }



                        // 3rd check if no handler method was found, throw an exception
                        if (geneHandlerMethod == null && strainHandlerMethod == null)
                        {
                            _logger.LogError("Handler method not found {name}", nameof(GeneEventConsumer));
                            throw new ArgumentNullException(nameof(GeneEventConsumer), "Handler method not found");
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