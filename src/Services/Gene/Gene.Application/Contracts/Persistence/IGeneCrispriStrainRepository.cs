
using Gene.Domain.Entities;
using Gene.Domain.EntityRevisions;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneCrispriStrainRepository
    {
        Task AddCrispriStrain(CrispriStrain crispriStrain);
        Task<CrispriStrain> Read(Guid id);
        Task<List<CrispriStrain>> GetCrispriStrainList();
        Task<List<CrispriStrain>> GetCrispriStrainOfGene(Guid geneId);
        Task UpdateCrispriStrain(CrispriStrain crispriStrain);
        Task DeleteCrispriStrain(Guid id);
        Task DeleteAllCrispriStrainsOfGene(Guid geneId);
        Task<CrispriStrainRevision> GetCrispriStrainRevisions(Guid Id);

    }
}