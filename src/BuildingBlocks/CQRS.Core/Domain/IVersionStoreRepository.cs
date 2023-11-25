using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain.Historical;

namespace CQRS.Core.Domain
{
    public interface IVersionStoreRepository<EntityModel> where EntityModel : BaseVersionEntity
    {
        Task<EntityModel> GetByAsyncId(Guid entityId);
        Task<EntityModel> GetByAsyncEntityId(Guid entityId);
        Task SaveAsync(EntityModel newModel);
        Task UpdateAsync(EntityModel newModel);
        
        
    }
}