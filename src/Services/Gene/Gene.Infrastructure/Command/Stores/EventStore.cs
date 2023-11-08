using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Gene.Domain.Aggregates;

namespace Gene.Infrastructure.Command.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        //private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;

        }
        // TODO: Uncomment this when Kafka is ready
        // public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        // {
        //     _eventStoreRepository = eventStoreRepository;
        //     _eventProducer = eventProducer;
        // }
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

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;

                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(GeneAggregate),
                    EventData = @event,
                    EventType = eventType,
                    Version = version
                };

                await _eventStoreRepository.SaveAsync(eventModel);

                // TODO: Uncomment this when Kafka is ready
                //var topic = $"{eventModel.AggregateType}-{eventModel.EventType}";
                // var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

                // await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}