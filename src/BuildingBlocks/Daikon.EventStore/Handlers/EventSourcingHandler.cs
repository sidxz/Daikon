using CQRS.Core.Domain;
using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Models;
using Daikon.EventStore.Repositories;
using Daikon.EventStore.Stores;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Daikon.EventStore.Handlers
{
    public class EventSourcingHandler<TAggregate> : IEventSourcingHandler<TAggregate>
        where TAggregate : AggregateRoot, new()
    {
        private readonly IEventStore<TAggregate> _eventStore;
        private readonly ISnapshotRepository _snapshotRepository;
        private readonly ILogger<EventSourcingHandler<TAggregate>> _logger;
        private readonly JsonSerializerSettings _jsonSettings;
        private const int SnapshotThreshold = 250;

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

        public async Task<TAggregate> GetByAsyncId(Guid aggregateId)
        {
            _logger.LogInformation("🔍 Loading aggregate with ID: {AggregateId}", aggregateId);
            var aggregate = new TAggregate();
            int snapshotVersion = -1;

            var snapshot = await _snapshotRepository.GetLatestSnapshotAsync(aggregateId);
            if (snapshot != null)
            {
                _logger.LogInformation("📦 Snapshot found for aggregate ID: {AggregateId}, version: {Version}", aggregateId, snapshot.Version);
                aggregate = JsonConvert.DeserializeObject<TAggregate>(snapshot.Data, _jsonSettings) ?? new TAggregate();
                snapshotVersion = snapshot.Version;
                aggregate.Version = snapshotVersion;
            }

            // ⚠️ ONLY apply events after the snapshot
            var events = await _eventStore.GetEventsAfterVersionAsync(aggregateId, snapshotVersion);
            aggregate.ReplayEvents(events);

            return aggregate;
        }


        public async Task SaveAsync(AggregateRoot aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var uncommitted = aggregate.GetUncommittedChanges().ToList();
            var expectedVersion = aggregate.Version;

            await _eventStore.SaveEventAsync(aggregate.Id, uncommitted, expectedVersion);

            _logger.LogInformation("📥 Saved {EventCount} events for aggregate ID: {AggregateId}", uncommitted.Count, aggregate.Id);

            if (aggregate is TAggregate agg && agg.Version % SnapshotThreshold == 0)
            {
                _logger.LogInformation("📸 Creating snapshot for aggregate ID: {AggregateId} at version {Version}", agg.Id, agg.Version);

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
                _logger.LogInformation("🛑 No snapshot created. Current version {Version} not divisible by threshold {Threshold}", aggregate.Version, SnapshotThreshold);
            }

            aggregate.MarkChangesAsCommitted();
        }
    }
}
