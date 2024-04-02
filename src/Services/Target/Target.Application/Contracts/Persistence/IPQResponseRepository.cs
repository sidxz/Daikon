using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Target.Application.Contracts.Persistence
{
    public interface IPQResponseRepository
    {
        Task<Domain.Entities.PQResponse> Create(Domain.Entities.PQResponse pqResponse);
        Task<Domain.Entities.PQResponse> ReadById(Guid id);
        Task<Domain.Entities.PQResponse> ReadByTargetId(Guid targetId);
        Task<List<Domain.Entities.PQResponse>> ReadAll();
        Task<List<Domain.Entities.PQResponse>> ReadPendingVerification();
        Task<List<Domain.Entities.PQResponse>> ReadApproved();
        
        Task<Domain.Entities.PQResponse> Update(Domain.Entities.PQResponse pqResponse);
        Task Delete(Guid id);
        
    }
}