using System.Reflection;
using Daikon.EventStore.Event;

namespace Daikon.EventStore.Aggregate
{
    /*
     Abstract base class representing the root of an aggregate in a CQRS + Event Sourcing architecture.
     Implements core event sourcing behavior: raising, applying, and replaying events.
    */
    public abstract class AggregateRoot
    {
        /* Unique identifier for the aggregate */
        protected Guid _id;

        /* Public getter for aggregate ID */
        public Guid Id => _id;

        /* Tracks uncommitted domain events */
        private readonly List<BaseEvent> _uncommittedChanges = new();

        /* Version of the aggregate, used for concurrency control */
        public int Version { get; set; } = -1;

        /* Caches Apply method delegates for each event type */
        private static readonly Dictionary<Type, Action<AggregateRoot, BaseEvent>> _applyMethodCache = new();

        /*
         Returns a read-only collection of uncommitted events.
        */
        public IEnumerable<BaseEvent> GetUncommittedChanges() => _uncommittedChanges.AsReadOnly();

        /*
         Clears the list of uncommitted changes after they have been persisted.
        */
        public void MarkChangesAsCommitted() => _uncommittedChanges.Clear();

        /*
         Raises a new domain event and applies it to the aggregate.
         The event is marked as "new" and added to the uncommitted changes.
        */
        protected void RaiseEvent(BaseEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event), "Event cannot be null.");

            ApplyChange(@event, isNew: true);
        }

        /*
         Rehydrates the aggregate state by replaying historical events.
         Events must be in correct sequential order (by version).
        */
        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events), "Event stream cannot be null.");

            var orderedEvents = events.OrderBy(e => e.Version).ToList();

            for (int i = 0; i < orderedEvents.Count; i++)
            {
                var @event = orderedEvents[i];

                /* Check for non-sequential event stream */
                if (i > 0 && orderedEvents[i - 1].Version + 1 != @event.Version)
                {
                    throw new InvalidOperationException(
                        $"Event stream is not sequential at index {i}. Expected version {orderedEvents[i - 1].Version + 1}, but got {@event.Version}.");
                }

                ApplyChange(@event, isNew: false);
                Version = @event.Version;
            }
        }

        /*
         Creates a shallow clone of the aggregate without uncommitted changes.
         Useful for projecting or comparing clean state.
        */
        public AggregateRoot CloneWithoutChanges()
        {
            var clone = (AggregateRoot)this.MemberwiseClone();

            /* Use reflection to reset the uncommitted changes list */
            var changesField = typeof(AggregateRoot)
                .GetField("_uncommittedChanges", BindingFlags.NonPublic | BindingFlags.Instance);

            changesField?.SetValue(clone, new List<BaseEvent>());
            return clone;
        }

        /*
         Applies an event to the aggregate using the appropriate Apply method.
         Uses reflection and caching for performance. If the event is new, it is added to the change list.
        */
        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event), "Event cannot be null.");

            var eventType = @event.GetType();

            /* Retrieve or build the Apply method delegate */
            if (!_applyMethodCache.TryGetValue(eventType, out var applyAction))
            {
                var methodInfo = GetType().GetMethod(
                    "Apply",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    new[] { eventType },
                    null);

                if (methodInfo == null)
                {
                    throw new NotImplementedException(
                        $"No Apply method found for event type '{eventType.FullName}'. Ensure you have a method like 'void Apply({eventType.Name} evt)' in your aggregate.");
                }

                applyAction = (aggregate, evt) => methodInfo.Invoke(aggregate, new[] { evt });
                _applyMethodCache[eventType] = applyAction;
            }

            /* Invoke the Apply method */
            applyAction(this, @event);

            /* If this is a new event, track it */
            if (isNew)
                _uncommittedChanges.Add(@event);
        }
    }
}
