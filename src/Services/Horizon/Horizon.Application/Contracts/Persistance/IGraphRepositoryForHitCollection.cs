
using Horizon.Domain.Screens;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForHitCollection
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddHitCollection(HitCollection hitCollection);
        Task UpdateHitCollection(HitCollection hitCollection);
        Task UpdateAssociatedScreenOfHitCollection(HitCollection hitCollection);
        Task DeleteHitCollection(string hitCollectionId);
        Task RenameHitCollection(string hitCollectionId, string newName);

        Task AddHit(Hit hit);
        Task UpdateHit(Hit hit);
        Task DeleteHit(string hitId);
        
    }
}