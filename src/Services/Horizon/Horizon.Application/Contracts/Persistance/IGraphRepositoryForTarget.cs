
using Horizon.Domain.Targets;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForTarget
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddTarget(Target target);
        Task UpdateTarget(Target target);
        Task UpdateAssociatedGenesOfTarget(Target target);
        Task RenameTarget(string targetId, string newName);
        Task DeleteTarget(string targetId);
    }
}