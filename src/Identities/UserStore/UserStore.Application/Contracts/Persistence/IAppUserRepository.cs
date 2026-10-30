
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

        Task<List<AppUser>> GetUsersByRole(Guid roleId);
        Task UpdateUser(AppUser user);
        // First sign-in: records the token identity on an unlocked account whose identity fields are still unset (or already
        // this value), touching nothing else. False when the account changed meanwhile: locked, linked elsewhere, or gone.
        Task<bool> LinkIdentity(Guid userId, string? entraObjectId, string? oidcSub);
        Task DeleteUser(Guid id);
    }
}