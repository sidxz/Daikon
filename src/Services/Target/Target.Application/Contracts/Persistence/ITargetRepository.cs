

namespace Target.Application.Contracts.Persistence
{
    public interface ITargetRepository
    {
        Task CreateTarget(Domain.Entities.Target target);
        Task<Domain.Entities.Target> ReadTargetById(Guid id);
        Task<Domain.Entities.Target> ReadTargetByName(string name);
        Task<List<Domain.Entities.Target>> GetTargetsList();
        Task<List<Domain.Entities.Target>> GetTargetsListByStrainId(Guid strainId);
        Task UpdateTarget(Domain.Entities.Target target);
        Task DeleteTarget(Domain.Entities.Target target);
    }
}