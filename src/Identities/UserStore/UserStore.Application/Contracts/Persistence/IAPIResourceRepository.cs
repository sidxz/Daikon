
using UserStore.Domain.Entities;

namespace UserStore.Application.Contracts.Persistence
{
    public interface IAPIResourceRepository
    {
        Task AddAPIResource(APIResource resource);
        Task<APIResource> GetAPIResourceById(Guid id);
        Task<APIResource> GetAPIResourceByEndPoint(string method, string endpoint);
        Task<List<APIResource>> GetAPIResourcesByService(string service);
        Task<List<APIResource>> GetAPIResourcesList();
        Task UpdateAPIResource(APIResource resource);
        Task DeleteAPIResource(Guid id);

    }
}