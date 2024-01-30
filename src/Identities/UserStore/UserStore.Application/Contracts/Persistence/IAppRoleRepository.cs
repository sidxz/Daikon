
using UserStore.Domain.Entities;

namespace UserStore.Application.Contracts.Persistence
{
    public interface IAppRoleRepository
    {
        Task AddRole(AppRole role);
        Task<AppRole> GetRoleById(Guid id);
        Task<AppRole> GetRoleByName(string name);
        Task<List<AppRole>> GetRolesList();
        Task UpdateRole(AppRole role);
        Task DeleteRole(Guid id);
       
    }
}