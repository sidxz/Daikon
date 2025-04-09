
using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Stores;

namespace Daikon.EventStore.Handlers
{
    /// <summary>
    /// Generic event sourcing handler for aggregates of type TAggregate.
    /// Responsible for loading and saving aggregate state using event sourcing techniques.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate root that the handler works with.</typeparam>
    public class EventSourcingHandler<TAggregate> : IEventSourcingHandler<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly IEventStore<TAggregate> _eventStore;

        public EventSourcingHandler(IEventStore<TAggregate> eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        /// <summary>
        /// Retrieves an aggregate by its identifier asynchronously.
        /// Replays all events for the aggregate to reconstruct its state.
        /// </summary>
        /// <param name="aggregateId">The unique identifier of the aggregate.</param>
        /// <returns>The fully reconstituted aggregate.</returns>
        public async Task<TAggregate> GetByAsyncId(Guid aggregateId)
        {
            // Create a new instance of the aggregate
            var aggregate = new TAggregate();

            // Retrieve events from the event store for the given aggregateId
            var events = await _eventStore.GetEventsAsync(aggregateId);
            if (events == null || !events.Any())
            {
                // No events found, return a default empty aggregate
                return aggregate;
            }

            // Replay events to rebuild aggregate state
            aggregate.ReplayEvents(events);

            // Set the version to the highest version from the events
            aggregate.Version = events.Max(e => e.Version);

            return aggregate;
        }

        /// <summary>
        /// Saves the state of an aggregate by persisting its uncommitted changes (events) to the event store.
        /// After saving, marks the changes as committed.
        /// </summary>
        /// <param name="aggregate">The aggregate whose state needs to be saved.</param>
        public async Task SaveAsync(AggregateRoot aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate), "Aggregate cannot be null when saving.");
            }

            // Save the uncommitted changes (events) to the event store
            await _eventStore.SaveEventAsync(
                aggregate.Id,
                aggregate.GetUncommittedChanges(),
                aggregate.Version
            );

            // Mark changes as committed after saving
            aggregate.MarkChangesAsCommitted();
        }
    }
}
