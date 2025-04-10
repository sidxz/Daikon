using Daikon.EventStore.Models;

namespace Daikon.EventStore.Repositories
{
    /*
     Defines the contract for managing aggregate snapshots in the event store.
     Used to optimize performance by avoiding full event stream replay.
    */
    public interface ISnapshotRepository
    {
        /*
         Retrieves the most recent snapshot for a given aggregate ID.

         Parameters:
         - aggregateId: The unique identifier of the aggregate.

         Returns:
         - A Task that resolves to the latest SnapshotModel, or null if not found.
        */
        Task<SnapshotModel?> GetLatestSnapshotAsync(Guid aggregateId);

        /*
         Persists a new snapshot into the snapshot store.

         Parameters:
         - snapshot: The snapshot to save.

         Returns:
         - A Task representing the asynchronous operation.
        */
        Task SaveSnapshotAsync(SnapshotModel snapshot);
    }
}
