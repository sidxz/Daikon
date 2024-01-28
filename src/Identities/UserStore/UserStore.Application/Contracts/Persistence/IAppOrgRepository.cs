
using UserStore.Domain.Entities;

namespace UserStore.Application.Contracts.Persistence
{
    public interface IAppOrgRepository
    {
        Task AddOrg(AppOrg org);
        Task<AppOrg> GetOrgById(Guid id);
        Task<AppOrg> GetOrgByName(string name);
        Task<List<AppOrg>> GetOrgsList();
        Task UpdateOrg(AppOrg org);
        Task DeleteOrg(Guid id);
    }
}