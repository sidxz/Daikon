
using UserStore.Domain.Entities;

namespace UserStore.Application.Contracts.Persistence
{
    public interface IAppUserRepository
    {
        Task AddUser(AppUser user);
        Task<AppUser> GetUserById(Guid id);
        Task<AppUser> GetUserByEmail(string email);
        Task<AppUser> GetUserByOIDCSub(string OIDCSub);
        Task<AppUser> GetUserByEntraObjectId(string EntraObjectId);
        Task<List<AppUser>> GetUsersList();
        Task UpdateUser(AppUser user);
        Task DeleteUser(Guid id);
    }
}