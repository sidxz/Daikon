
using Gene.Domain.Entities;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneResistanceMutationRepository
    {
        Task AddResistanceMutation(ResistanceMutation resistanceMutation);
        Task<ResistanceMutation> Read(Guid id);
        Task<List<ResistanceMutation>> GetResistanceMutationList();
        Task<List<ResistanceMutation>> GetResistanceMutationOfGene(Guid geneId);
        Task UpdateResistanceMutation(ResistanceMutation resistanceMutation);
        Task DeleteResistanceMutation(Guid id);
        Task DeleteAllResistanceMutationsOfGene(Guid geneId);
    }
}