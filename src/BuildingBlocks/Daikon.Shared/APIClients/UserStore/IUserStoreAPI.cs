
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using Daikon.Shared.VM.UserStore;

namespace Daikon.Shared.APIClients.UserStore
{
    public interface IUserStoreAPI
    {
        public Task<AppUserVM> GetUserById(Guid id, bool forceRefresh = false);
        public Task<List<AppUserVM>> GetUsers(bool forceRefresh = false);
        public Task<AppOrgVM> GetOrgById(Guid id, bool forceRefresh = false);
        public Task<List<AppOrgVM>> GetOrgs(bool forceRefresh = false);
    }
}