
using Horizon.Domain.Genes;
using Horizon.Domain.Strains;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForGene
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddGene(Gene gene);
        Task UpdateGene(Gene gene);
        Task DeleteGene(string geneId);
        Task AddStrain(Strain strain);
        Task UpdateStrain(Strain strain);
    }
}