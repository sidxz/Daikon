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
        private const int SnapshotThreshold = 5;

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
                // log the deserialized aggregate
                _logger.LogInformation("📦 Deserialized aggregate: {Aggregate}", JsonConvert.SerializeObject(aggregate, _jsonSettings));
                snapshotVersion = snapshot.Version;
                aggregate.Version = snapshotVersion;
            }

            /* Replay events that occurred after the snapshot version */
            var events = await _eventStore.GetEventsAfterVersionAsync(aggregateId, snapshotVersion);


            // 🚨 Guardrail check for version mismatch
            if (events.Any() && events.First().Version != snapshotVersion + 1)
            {
                _logger.LogWarning(
                    "🚨 Event stream version mismatch for aggregate ID {AggregateId}. " +
                    "Expected next version: {ExpectedVersion}, but found: {ActualVersion}.",
                    aggregateId,
                    snapshotVersion + 1,
                    events.First().Version
                );
            }


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

            /*
                Why expectedVersion is the standard name
                In event sourcing, the concept of versioning is centered around:
                “What is the version of the aggregate that the caller believes to be current?”
                This version is passed to the event store which verifies  what is in the database, updates the version, and saves the events.
                If the version in the database is not the same as the one passed, it means that another process has modified the aggregate.
                This is a concurrency control mechanism to ensure that the caller is working with the latest version of the aggregate.
            */
            var expectedVersion = aggregate.Version;

            await _eventStore.SaveEventAsync(aggregate.Id, uncommitted, expectedVersion);

            _logger.LogInformation("📥 Saved {EventCount} events for aggregate ID: {AggregateId}", uncommitted.Count, aggregate.Id);

            /*
             If aggregate has reached snapshot threshold, create a new snapshot.
             This helps optimize future loads.
            */
            if (aggregate is TAggregate agg)
            {

                var latestVersion = uncommitted.LastOrDefault()?.Version ?? aggregate.Version;

                var crossedSnapshotVersions = Enumerable
                        .Range(expectedVersion + 1, latestVersion - expectedVersion)
                        .Where(v => v % SnapshotThreshold == 0)
                        .ToList();
                _logger.LogInformation("📸 Crossed snapshot versions: {Versions}", string.Join(", ", crossedSnapshotVersions));

                if (crossedSnapshotVersions.Any())
                {
                    /* Create a clean copy of the aggregate with no uncommitted events */
                    var cleanAggregate = (TAggregate)agg.CloneWithoutChanges();
                    var snapshotVersion = latestVersion; // Snapshot the latest version

                    cleanAggregate.Version = snapshotVersion;

                    _logger.LogInformation("📸 Creating snapshot for aggregate ID: {AggregateId} at version {Version}", agg.Id, snapshotVersion);
                    // log both aggVersion and latestVersion


                    var snapshot = new SnapshotModel
                    {
                        AggregateIdentifier = agg.Id,
                        AggregateType = typeof(TAggregate).Name,
                        Data = JsonConvert.SerializeObject(cleanAggregate, _jsonSettings),
                        Version = cleanAggregate.Version,
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
            }


            /* Mark changes as committed to clear uncommitted event list */
            aggregate.MarkChangesAsCommitted();
        }
    }
}
