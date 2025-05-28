using Daikon.EventStore.Models;

namespace Daikon.EventStore.Repositories
{
    /*
     Defines the contract for interacting with the event store.
     Supports operations for saving and retrieving domain events.
    */
    public interface IEventStoreRepository
    {
        /*
         Saves a single event asynchronously into the event store.

         Parameters:
         - event: The event to be persisted.

         Returns:
         - A Task representing the asynchronous operation.
        */
        Task SaveAsync(EventModel @event);

        /*
         Retrieves all events for a given aggregate ID, ordered by version.

         Parameters:
         - aggregateId: The identifier of the aggregate.

         Returns:
         - A Task containing a list of matching events.
        */
        Task<List<EventModel>> FindByAggregateId(Guid aggregateId);

        /*
         Saves multiple events in a single batch operation.

         Parameters:
         - events: The collection of events to save.

         Returns:
         - A Task representing the asynchronous operation.
        */
        Task SaveBatchAsync(IEnumerable<EventModel> events);

        /*
         Retrieves all aggregate IDs stored in the event store.

         Returns:
         - A Task containing a collection of aggregate IDs.
        */
        Task<IEnumerable<Guid>> GetAllAggregateIds();
    }
}
