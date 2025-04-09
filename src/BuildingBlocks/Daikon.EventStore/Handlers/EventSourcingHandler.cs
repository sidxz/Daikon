using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Models;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Stores;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Daikon.EventStore.Handlers
{
    /*
     Generic event sourcing handler responsible for loading, saving, and snapshotting aggregates.
     Works with any aggregate type that inherits from AggregateRoot.
    */
    public class EventSourcingHandler<TAggregate> : IEventSourcingHandler<TAggregate>
        where TAggregate : AggregateRoot, new()
    {
        private readonly IEventStore<TAggregate> _eventStore;
        private readonly ISnapshotRepository _snapshotRepository;
        private readonly ILogger<EventSourcingHandler<TAggregate>> _logger;
        private readonly JsonSerializerSettings _jsonSettings;

        /* Snapshot threshold - a snapshot is created every 250 events */
        private const int SnapshotThreshold = 250;

        /*
         Constructor initializes dependencies and sets up JSON serialization settings
        */
        public EventSourcingHandler(
            IEventStore<TAggregate> eventStore,
            ISnapshotRepository snapshotRepository,
            ILogger<EventSourcingHandler<TAggregate>> logger)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    DefaultMembersSearchFlags = System.Reflection.BindingFlags.Public |
                                                System.Reflection.BindingFlags.NonPublic |
                                                System.Reflection.BindingFlags.Instance
                },
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.None
            };
        }

        /*
         Loads an aggregate from the event store (and snapshot if available) by its ID.
         Replays events that occurred after the latest snapshot to bring it to the current state.
        */
        public async Task<TAggregate> GetByAsyncId(Guid aggregateId)
        {
            _logger.LogInformation("🔍 Loading aggregate with ID: {AggregateId}", aggregateId);

            var aggregate = new TAggregate();
            int snapshotVersion = -1;

            /* Attempt to load the latest snapshot */
            var snapshot = await _snapshotRepository.GetLatestSnapshotAsync(aggregateId);
            if (snapshot != null)
            {
                _logger.LogInformation("📦 Snapshot found for aggregate ID: {AggregateId}, version: {Version}", aggregateId, snapshot.Version);

                aggregate = JsonConvert.DeserializeObject<TAggregate>(snapshot.Data, _jsonSettings) ?? new TAggregate();
                snapshotVersion = snapshot.Version;
                aggregate.Version = snapshotVersion;
            }

            /* Replay events that occurred after the snapshot version */
            var events = await _eventStore.GetEventsAfterVersionAsync(aggregateId, snapshotVersion);
            aggregate.ReplayEvents(events);

            return aggregate;
        }

        /*
         Persists the uncommitted changes of an aggregate to the event store.
         Also triggers snapshot creation when threshold is met.
        */
        public async Task SaveAsync(AggregateRoot aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var uncommitted = aggregate.GetUncommittedChanges().ToList();
            var expectedVersion = aggregate.Version;

            await _eventStore.SaveEventAsync(aggregate.Id, uncommitted, expectedVersion);

            _logger.LogInformation("📥 Saved {EventCount} events for aggregate ID: {AggregateId}", uncommitted.Count, aggregate.Id);

            /*
             If aggregate has reached snapshot threshold, create a new snapshot.
             This helps optimize future loads.
            */
            if (aggregate is TAggregate agg && agg.Version % SnapshotThreshold == 0)
            {
                _logger.LogInformation("📸 Creating snapshot for aggregate ID: {AggregateId} at version {Version}", agg.Id, agg.Version);

                /* Create a clean copy of the aggregate with no uncommitted events */
                var cleanAggregate = (TAggregate)agg.CloneWithoutChanges();

                var snapshot = new SnapshotModel
                {
                    AggregateIdentifier = agg.Id,
                    AggregateType = typeof(TAggregate).Name,
                    Data = JsonConvert.SerializeObject(cleanAggregate, _jsonSettings),
                    Version = agg.Version,
                    TimeStamp = DateTime.UtcNow
                };

                await _snapshotRepository.SaveSnapshotAsync(snapshot);
            }
            else
            {
                _logger.LogInformation(
                    "🛑 No snapshot created. Current version {Version} not divisible by threshold {Threshold}",
                    aggregate.Version,
                    SnapshotThreshold);
            }

            /* Mark changes as committed to clear uncommitted event list */
            aggregate.MarkChangesAsCommitted();
        }
    }
}
