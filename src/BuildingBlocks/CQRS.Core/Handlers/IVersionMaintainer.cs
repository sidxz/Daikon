using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Domain.Historical;

namespace CQRS.Core.Handlers
{
    public interface IVersionMaintainer<VersionEntityModel> where VersionEntityModel : BaseVersionEntity
    {
        Task CommitVersion(BaseEntity updatedEntity);
        Task<VersionEntityModel> GetVersions(Guid entityId);
        
    }
}