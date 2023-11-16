using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Daikon.EventStore.Settings;

namespace Daikon.EventStore.Stores
{
    public class EventStore<TAggregate> : IEventStore<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        private readonly IEventProducer _eventProducer;

        private readonly IKafkaProducerSettings _kafkaProducerSettings;

        private const string DefaultKafkaTopic = "default_topic";


        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer, IKafkaProducerSettings kafkaProducerSettings)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
            _kafkaProducerSettings = kafkaProducerSettings;
        }
        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
            if (!eventStream.Any())
            {
                throw new AggregateNotFoundException($"Aggregate with id {aggregateId} not found");
            }

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();

        }

        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException($"Aggregate {aggregateId} has been modified since last read");
            }

            var version = expectedVersion;

            var eventModels = new List<EventModel>();

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;

                var eventModel = ConvertToEventModel(@event, aggregateId, version);
                eventModels.Add(eventModel);
            }

            await _eventStoreRepository.SaveBatchAsync(eventModels);
            foreach (var eventModel in eventModels)
            {
                await ProduceEvent(eventModel.EventData);
            }

        }

        private EventModel ConvertToEventModel(
            BaseEvent @event, Guid aggregateId, int version,
            string? userId = null, string? sessionId = null, string? source = null,
            Guid correlationId = default, Guid causationId = default, string? metadata = null,
            string? tenantId = null, string? eventState = null
            )
        {
            return new EventModel
            {
                TimeStamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = typeof(TAggregate).Name,
                EventData = @event,
                EventType = @event.GetType().Name,
                Version = version,
                UserId = userId,
                SessionId = sessionId,
                Source = source,
                CorrelationId = correlationId,
                CausationId = causationId,
                Metadata = metadata,
                TenantId = tenantId,
                EventState = eventState,
            };
        }

        private async Task ProduceEvent(BaseEvent @event)
        {
            var topic = _kafkaProducerSettings.Topic ?? DefaultKafkaTopic;
            if (string.IsNullOrEmpty(topic))
            {
                // Log a warning or notify administrators
                // Continue to produce event with default topic or handle as per application logic
                throw new Exception("KAFKA_TOPIC environment variable not set");
            }
            await _eventProducer.ProduceAsync<BaseEvent>(topic, @event);
        }


    }
}