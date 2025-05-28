using Daikon.EventStore.Event;
using Daikon.EventStore.Settings;
using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Producers;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Models;

namespace Daikon.EventStore.Stores
{
    /*
     Generic EventStore implementation that supports saving and retrieving events for a given aggregate type.
     Includes support for optimistic concurrency control and event publication to Kafka.
    */
    public class EventStore<TAggregate> : IEventStore<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;
        private readonly IKafkaProducerSettings _kafkaProducerSettings;

        private const string DefaultKafkaTopic = "default_topic";

        /*
         Constructor to initialize dependencies.
        */
        public EventStore(
            IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer,
            IKafkaProducerSettings kafkaProducerSettings)
        {
            _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
            _kafkaProducerSettings = kafkaProducerSettings ?? throw new ArgumentNullException(nameof(kafkaProducerSettings));
        }

        /*
         Retrieves all events for a given aggregate ID.
         Throws AggregateNotFoundException if no events exist.
        */
        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (!eventStream.Any())
            {
                throw new AggregateNotFoundException($"Aggregate with ID {aggregateId} not found");
            }

            return eventStream
                .OrderBy(x => x.Version)
                .Select(x => x.EventData)
                .ToList();
        }

        /*
         Saves a batch of events to the store and publishes them to Kafka.
         Validates expected version to ensure concurrency safety.
         Throws ConcurrencyException if there is a version conflict.
        */
        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var existingEvents = await _eventStoreRepository.FindByAggregateId(aggregateId);
            var lastVersion = existingEvents.LastOrDefault()?.Version ?? -1;



            /* Check if the expected version matches the last version in the store
            This is a safety mechanism to prevent conflicting writes.

                Example:

                Client A loads version 3 of the aggregate.

                Client B loads version 3 too.

                Client A saves an event (now version 4).

                Client B tries to save an event based on version 3 — this check fails and throws.

                ✅ This ensures only one client can modify the aggregate at a time.
            
             */

             
            if (expectedVersion != -1 && lastVersion != expectedVersion)
            {
                throw new ConcurrencyException(
                    $"Aggregate {aggregateId} has been modified. Expected version {expectedVersion}, but found {lastVersion}.");
            }

            var version = expectedVersion;
            var orderedEvents = events.ToList();

            /* Assign version numbers to new events */
            foreach (var @event in orderedEvents)
            {
                version++;
                @event.Version = version;
            }

            /* Persist events to the database */
            var eventModels = orderedEvents.Select(e =>
                ConvertToEventModel(e, aggregateId, e.Version)).ToList();

            await _eventStoreRepository.SaveBatchAsync(eventModels);

            /* Publish each event to the Kafka topic */
            foreach (var @event in orderedEvents)
            {
                await ProduceEvent(@event);
            }
        }

        /*
         Retrieves all events after a specific version for a given aggregate.
         Typically used when applying events after a snapshot.
        */
        public async Task<List<BaseEvent>> GetEventsAfterVersionAsync(Guid aggregateId, int version)
        {
            var allEvents = await _eventStoreRepository.FindByAggregateId(aggregateId);

            return allEvents
                .Where(x => x.Version > version)
                .OrderBy(x => x.Version)
                .Select(x => x.EventData)
                .ToList();
        }

        /*
         Converts a domain event into an EventModel suitable for persistence.
         Optionally accepts metadata such as user/session context, correlation IDs, etc.
        */
        private EventModel ConvertToEventModel(
            BaseEvent @event,
            Guid aggregateId,
            int version,
            string? userId = null,
            string? sessionId = null,
            string? source = null,
            Guid correlationId = default,
            Guid causationId = default,
            string? metadata = null,
            string? tenantId = null,
            string? eventState = null)
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
                EventState = eventState
            };
        }

        /*
         Publishes the event to the configured Kafka topic.
         Throws InvalidOperationException if the topic is missing.
        */
        private async Task ProduceEvent(BaseEvent @event)
        {
            var topic = _kafkaProducerSettings.Topic ?? DefaultKafkaTopic;

            if (string.IsNullOrEmpty(topic))
            {
                throw new InvalidOperationException("Kafka topic is not configured.");
            }

            await _eventProducer.ProduceAsync(topic, @event);
        }
    }
}
