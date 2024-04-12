
using Gene.Domain.Entities;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneExpansionPropRepo
    {
        public Task Create(GeneExpansionProp geneExpansionProps);
        public Task Update(GeneExpansionProp geneExpansionProps);
        public Task<GeneExpansionProp> ReadById(Guid id);
        public Task<List<GeneExpansionProp>> ListByEntityId(Guid entityId);
        public Task DeleteAllOfEntity(Guid entityId);
        public Task Delete(Guid id);
        
    }
}