
using Gene.Domain.Entities;
using Gene.Domain.EntityRevisions;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneEssentialityRepository
    {
        Task AddEssentiality(Essentiality essentiality);
        Task<Essentiality> Read(Guid id);
        Task<List<Essentiality>> GetEssentialityList();
        Task<List<Essentiality>> GetEssentialityOfGene(Guid geneId);
        Task UpdateEssentiality(Essentiality essentiality);
        Task DeleteEssentiality(Guid id);
        Task DeleteAllEssentialitiesOfGene(Guid geneId);
        Task<EssentialityRevision> GetEssentialityRevisions(Guid Id);

    }
}