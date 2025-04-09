using Daikon.EventStore.Models;

namespace Daikon.EventStore.Repositories
{
    public interface ISnapshotRepository
    {
        Task<SnapshotModel?> GetLatestSnapshotAsync(Guid aggregateId);
        Task SaveSnapshotAsync(SnapshotModel snapshot);
    }
}
