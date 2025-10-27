using CQRS.Core.Exceptions;
using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Models;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Stores;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Daikon.EventStore.Handlers
{
    /*
     * Generic event sourcing handler responsible for managing the lifecycle of an aggregate.
     * Supports event replay, snapshot recovery, and persistence.
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
         * Retrieves an aggregate by ID, using snapshot and event replay for reconstruction.
         */
        public async Task<TAggregate> GetByAsyncId(Guid aggregateId)
        {
            _logger.LogInformation("🔍 Loading aggregate with ID: {AggregateId}", aggregateId);

            TAggregate aggregate;
            int snapshotVersion = -1;

            /* Attempt to load the latest snapshot */
            var snapshot = await _snapshotRepository.GetLatestSnapshotAsync(aggregateId);
            try
            {
                if (snapshot != null)
                {
                    _logger.LogInformation("📦 Snapshot found (ID: {AggregateId}, Version: {Version})", aggregateId, snapshot.Version);

                    aggregate = JsonConvert.DeserializeObject<TAggregate>(snapshot.Data, _jsonSettings) ?? new TAggregate();
                    //_logger.LogDebug("📦 Deserialized aggregate: {Aggregate}", JsonConvert.SerializeObject(aggregate, _jsonSettings));

                    snapshotVersion = snapshot.Version;
                    aggregate.Version = snapshotVersion;
                }

                else
                {
                    aggregate = new TAggregate();
                }

                /* Replay events that occurred after the snapshot version */
                var subsequentEvents = await _eventStore.GetEventsAfterVersionAsync(aggregateId, snapshotVersion);


                if (snapshot == null && !subsequentEvents.Any())
                    throw new AggregateNotFoundException($"Aggregate with ID {aggregateId} not found");


                // 🚨 Guardrail check for version mismatch
                if (subsequentEvents.Any() && subsequentEvents.First().Version != snapshotVersion + 1)
                {
                    _logger.LogWarning(
                        "🚨 Event version mismatch (ID: {AggregateId}, Expected: {Expected}, Actual: {Actual})",
                        aggregateId, snapshotVersion + 1, subsequentEvents.First().Version);

                }


                aggregate.ReplayEvents(subsequentEvents);

                return aggregate;
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Failed to deserialize snapshot for aggregate ID: {AggregateId}", aggregateId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load snapshot for aggregate ID: {AggregateId}", aggregateId);
                throw;
            }
        }

        /*
          * Saves uncommitted events of the aggregate to the event store.
          * Creates a snapshot if snapshot threshold is crossed.
          */
        public async Task SaveAsync(AggregateRoot aggregate)
        {
            ArgumentNullException.ThrowIfNull(aggregate);

            var uncommittedEvents = aggregate.GetUncommittedChanges().ToList();

            /*
                Why expectedVersion is the standard name
                In event sourcing, the concept of versioning is centered around:
                “What is the version of the aggregate that the caller believes to be current?”
                This version is passed to the event store which verifies  what is in the database, updates the version, and saves the events.
                If the version in the database is not the same as the one passed, it means that another process has modified the aggregate.
                This is a concurrency control mechanism to ensure that the caller is working with the latest version of the aggregate.
            */
            var expectedVersion = aggregate.Version;

            try
            {
                await _eventStore.SaveEventAsync(aggregate.Id, uncommittedEvents, expectedVersion);
                _logger.LogInformation("📥 Saved {EventCount} events for aggregate ID: {AggregateId}", uncommittedEvents.Count, aggregate.Id);
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "❌ Concurrency conflict saving aggregate {AggregateId}", aggregate.Id);
                throw;
            }

            /*
             If aggregate has reached snapshot threshold, create a new snapshot.
             This helps optimize future loads.
            */
            if (aggregate is TAggregate agg)
            {

                var latestEventVersion = uncommittedEvents.LastOrDefault()?.Version ?? aggregate.Version;

                var crossedThresholds = Enumerable
                        .Range(expectedVersion + 1, latestEventVersion - expectedVersion)
                        .Where(v => v % SnapshotThreshold == 0)
                        .ToList();
                //_logger.LogInformation("📸 Crossed snapshot versions: {Versions}", string.Join(", ", crossedThresholds));

                if (crossedThresholds.Any())
                {
                    /* Create a clean copy of the aggregate with no uncommitted events */
                    var cleanAggregate = (TAggregate)agg.CloneWithoutChanges();
                    cleanAggregate.Version = latestEventVersion;

                    _logger.LogInformation("📸 Creating snapshot for aggregate ID: {AggregateId} at version {Version}", agg.Id, cleanAggregate.Version);
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
                    // _logger.LogInformation(
                    //     "🛑 No snapshot created. Current version {Version} not divisible by threshold {Threshold}",
                    //     latestEventVersion,
                    //     SnapshotThreshold);
                }
            }


            /* Mark changes as committed to clear uncommitted event list */
            aggregate.MarkChangesAsCommitted();
        }
    }
}
