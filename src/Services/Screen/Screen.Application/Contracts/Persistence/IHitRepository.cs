
namespace Screen.Application.Contracts.Persistence
{
    public interface IHitRepository
    {
        Task CreateHit(Domain.Entities.Hit screen);
        Task<Domain.Entities.Hit> ReadHitById(Guid id);
        Task<List<Domain.Entities.Hit>> GetHitsList();
        Task<List<Domain.Entities.Hit>> GetHitsListByStrainId(Guid strainId);
        Task UpdateHit(Domain.Entities.Hit screen);
        Task DeleteHit(Domain.Entities.Hit screen);
        Task<Domain.EntityRevisions.HitRevision> GetHitRevisions(Guid Id);
    }
}