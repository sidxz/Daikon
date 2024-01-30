
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Infrastructure.Repositories
{
    public class AppRoleRepository : IAppRoleRepository
    {
        public Task AddRole(AppRole role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRole(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AppRole> GetRoleById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AppRole> GetRoleByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<AppRole>> GetRolesList()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRole(AppRole role)
        {
            throw new NotImplementedException();
        }
    }
}