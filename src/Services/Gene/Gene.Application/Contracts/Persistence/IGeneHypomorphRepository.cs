
using Gene.Domain.Entities;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneHypomorphRepository
    {
        Task AddHypomorph(Hypomorph hypomorph);
        Task<Hypomorph> Read(Guid id);
        Task<List<Hypomorph>> GetHypomorphList();
        Task<List<Hypomorph>> GetHypomorphOfGene(Guid geneId);
        Task UpdateHypomorph(Hypomorph hypomorph);
        Task DeleteHypomorph(Guid id);
        Task DeleteAllHypomorphsOfGene(Guid geneId);
    }
}