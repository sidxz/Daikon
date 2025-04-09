
using Gene.Domain.Entities;

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

    }
}