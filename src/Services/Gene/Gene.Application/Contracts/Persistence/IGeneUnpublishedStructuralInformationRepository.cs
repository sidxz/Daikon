
using Gene.Domain.Entities;
using Gene.Domain.EntityRevisions;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneUnpublishedStructuralInformationRepository
    {
        Task AddUnpublishedStructuralInformation(UnpublishedStructuralInformation essentiality);
        Task<UnpublishedStructuralInformation> Read(Guid id);
        Task<List<UnpublishedStructuralInformation>> GetUnpublishedStructuralInformationList();
        Task<List<UnpublishedStructuralInformation>> GetUnpublishedStructuralInformationOfGene(Guid geneId);
        Task UpdateUnpublishedStructuralInformation(UnpublishedStructuralInformation essentiality);
        Task DeleteUnpublishedStructuralInformation(Guid id);
        Task DeleteAllUnpublishedStructuralInformationsOfGene(Guid geneId);
        Task<UnpublishedStructuralInformationRevision> GetUnpublishedStructuralInformationRevisions(Guid Id);

    }
}