
using Horizon.Domain.Targets;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForTarget
    {
        Task CreateIndexesAsync();
        Task AddTarget(Target target);
        Task UpdateTarget(Target target);
        Task UpdateAssociatedGenesOfTarget(Target target);
        Task DeleteTarget(string targetId);
    }
}