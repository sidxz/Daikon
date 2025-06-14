
using Screen.Domain.Entities;

namespace Screen.Application.Contracts.Persistence
{
    public interface IHitRepository
    {
        Task CreateHit(Domain.Entities.Hit hit);
        Task<Domain.Entities.Hit> ReadHitById(Guid id);
        Task<List<Domain.Entities.Hit>> GetHitsList();
        Task<List<Domain.Entities.Hit>> GetHitsListByHitCollectionId(Guid hitCollectionId);
        Task UpdateHit(Domain.Entities.Hit hit);
        Task DeleteHit(Guid hitId);
        Task DeleteHitsByHitCollectionId(Guid hitCollectionId);
        Task DeleteHits(List<Guid> hitIds);

        Task<List<Hit>> GetHitsWithRequestedMoleculeNameButNoMoleculeId();
    }
}