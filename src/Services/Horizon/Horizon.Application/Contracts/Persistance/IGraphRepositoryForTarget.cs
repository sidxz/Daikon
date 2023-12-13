
using Horizon.Domain.Targets;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForTarget
    {
        Task CreateIndexesAsync();
        Task AddTargetToGraph(Target target);
        Task UpdateTargetOfGraph(Target target);
        Task DeleteTargetFromGraph(string targetId);
    }
}