using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Target.Application.Contracts.Persistence
{
    public interface IToxicologyRepo 
    {
        Task<Domain.Entities.Toxicology> Create(Domain.Entities.Toxicology toxicology);
        Task<Domain.Entities.Toxicology> ReadById(Guid id);
        Task<List<Domain.Entities.Toxicology>> ReadByTargetId(Guid targetId);
        Task<List<Domain.Entities.Toxicology>> ReadAll();
        Task<Domain.Entities.Toxicology> Update(Domain.Entities.Toxicology toxicology);
        Task Delete(Guid id);
        Task DeleteByTargetId(Guid targetId);
    }
}