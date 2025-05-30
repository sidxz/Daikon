
using Gene.Domain.Entities;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneProteinActivityAssayRepository
    {
        Task AddProteinActivityAssay(ProteinActivityAssay proteinActivityAssay);
        Task<ProteinActivityAssay> Read(Guid id);
        Task<List<ProteinActivityAssay>> GetProteinActivityAssayList();
        Task<List<ProteinActivityAssay>> GetProteinActivityAssayOfGene(Guid geneId);
        Task UpdateProteinActivityAssay(ProteinActivityAssay proteinActivityAssay);
        Task DeleteProteinActivityAssay(Guid id);
        Task DeleteAllProteinActivityAssaysOfGene(Guid geneId);

    }
}