using Daikon.EventStore.Aggregate;
using Daikon.EventStore.Event;

namespace Daikon.EventStore.Stores
{
    /*
     Defines the contract for an event store that handles persistence and retrieval of domain events
     for a specific aggregate type using event sourcing principles.
    */
    public interface IEventStore<TAggregate> where TAggregate : AggregateRoot
    {
        /*
         Saves a batch of domain events for the specified aggregate.

         Parameters:
         - aggregateId: The unique identifier of the aggregate.
         - events: The list of domain events to save.
         - expectedVersion: The version the aggregate is expected to be at (used for optimistic concurrency).

         Throws:
         - ConcurrencyException if the version check fails.
         - Exception if saving fails.
        */
        Task SaveEventAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);

        /*
         Retrieves the full list of events for the specified aggregate.

         Parameters:
         - aggregateId: The ID of the aggregate.

         Returns:
         - A Task containing the ordered list of domain events.

         Throws:
         - AggregateNotFoundException if no events are found.
        */
        Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);

        /*
         Retrieves all events for the aggregate that occurred after the specified version.

         Parameters:
         - aggregateId: The aggregate ID to retrieve events for.
         - version: The starting version (exclusive).

         Returns:
         - A Task containing the list of newer events.
        */
        Task<List<BaseEvent>> GetEventsAfterVersionAsync(Guid aggregateId, int version);
    }
}
