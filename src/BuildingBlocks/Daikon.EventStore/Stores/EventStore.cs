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

        public EventStore(
            IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer,
            IKafkaProducerSettings kafkaProducerSettings)
        {
            _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
            _kafkaProducerSettings = kafkaProducerSettings ?? throw new ArgumentNullException(nameof(kafkaProducerSettings));
        }

        /// <summary>
        /// Asynchronously retrieves a list of events for a specified aggregate ID.
        /// Throws AggregateNotFoundException if no events are found for the given aggregate ID.
        /// </summary>
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

        /// <summary>
        /// Asynchronously saves a batch of events for a specified aggregate ID.
        /// Ensures concurrency control by checking the expected version against the latest version in the event stream.
        /// Throws ConcurrencyException if there is a version mismatch.
        /// </summary>
        public async Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream.Any() && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException($"Aggregate {aggregateId} has been modified since the last read.");
            }

            var version = expectedVersion;
            var eventModels = events.Select(@event =>
            {
                version++;
                @event.Version = version;
                return ConvertToEventModel(@event, aggregateId, version);
            }).ToList();

            await _eventStoreRepository.SaveBatchAsync(eventModels);
            foreach (var eventModel in eventModels)
            {
                await ProduceEvent(eventModel.EventData);
            }
        }

        /// <summary>
        /// Converts a BaseEvent to an EventModel with additional metadata.
        /// </summary>
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

        /// <summary>
        /// Produces an event to the Kafka topic.
        /// Throws an exception if the Kafka topic is not configured.
        /// </summary>
        private async Task ProduceEvent(BaseEvent @event)
        {
            var topic = _kafkaProducerSettings.Topic ?? DefaultKafkaTopic;
            if (string.IsNullOrEmpty(topic))
            {
                // Log a warning or notify administrators
                throw new InvalidOperationException("Kafka topic is not configured.");
            }

            await _eventProducer.ProduceAsync(topic, @event);
        }
    }
}
