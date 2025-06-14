
using Gene.Domain.Entities;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneProteinProductionRepository
    {
        Task AddProteinProduction(ProteinProduction proteinProduction);
        Task<ProteinProduction> Read(Guid id);
        Task<List<ProteinProduction>> GetProteinProductionList();
        Task<List<ProteinProduction>> GetProteinProductionOfGene(Guid geneId);
        Task UpdateProteinProduction(ProteinProduction proteinProduction);
        Task DeleteProteinProduction(Guid id);
        Task DeleteAllProteinProductionsOfGene(Guid geneId);

    }
}