
using Horizon.Domain.Screens;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForHitCollection
    {
        Task CreateIndexesAsync();
        Task AddHitCollectionToGraph(HitCollection hitCollection);
        Task UpdateHitCollectionOfGraph(HitCollection hitCollection);
        Task UpdateAssociatedScreenOfHitCollection(HitCollection hitCollection);
        Task DeleteHitCollectionFromGraph(string hitCollectionId);
        
    }
}