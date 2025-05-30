using Daikon.EventStore.Aggregate;

namespace Daikon.EventStore.Handlers
{
    /*
     Defines a contract for applying and saving domain events using event sourcing.
     Responsible for loading aggregates and persisting their uncommitted changes.
    */
    public interface IEventSourcingHandler<TAggregate> where TAggregate : AggregateRoot
    {
        /*
         Persists the uncommitted changes of the provided aggregate to the event store.
         Also responsible for snapshotting when applicable.

         Parameters:
         - Root: The aggregate containing uncommitted events to be saved.

         Returns:
         - A Task representing the asynchronous operation.
        */
        Task SaveAsync(AggregateRoot Root);

        /*
         Loads an aggregate by ID, replaying its event history (and snapshot if available).

         Parameters:
         - id: The unique identifier of the aggregate to load.

         Returns:
         - A Task that resolves to the reconstructed aggregate.
        */
        Task<TAggregate> GetByAsyncId(Guid id);
    }
}
