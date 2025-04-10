
namespace Screen.Application.Contracts.Persistence
{
    public interface IHitCollectionRepository
    {
        Task CreateHitCollection(Domain.Entities.HitCollection hitCollection);
        Task<Domain.Entities.HitCollection> ReadHitCollectionByName(string name);
        Task<Domain.Entities.HitCollection> ReadHitCollectionById(Guid id);

        Task<List<Domain.Entities.HitCollection>> GetHitCollectionsList();
        Task<List<Domain.Entities.HitCollection>> GetHitCollectionsListByScreenId(Guid screenId);
        Task UpdateHitCollection(Domain.Entities.HitCollection hitCollection);
        Task DeleteHitCollection(Guid id);
    }
}