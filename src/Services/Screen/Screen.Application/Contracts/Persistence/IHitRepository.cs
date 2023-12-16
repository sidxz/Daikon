
namespace Screen.Application.Contracts.Persistence
{
    public interface IHitRepository
    {
        Task CreateHit(Domain.Entities.Hit hit);
        Task<Domain.Entities.Hit> ReadHitById(Guid id);
        Task<List<Domain.Entities.Hit>> GetHitsList();
        Task<List<Domain.Entities.Hit>> GetHitsListByHitCollectionId(Guid hitCollectionId);
        Task UpdateHit(Domain.Entities.Hit hit);
        Task DeleteHit(Domain.Entities.Hit hit);
        Task DeleteHitsByHitCollectionId(Guid hitCollectionId);
        Task DeleteHits(List<Guid> hitIds);
        Task<Domain.EntityRevisions.HitRevision> GetHitRevisions(Guid Id);
    }
}