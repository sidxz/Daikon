
using OcelotApiGw.DTOs;

namespace OcelotApiGw.Contracts.Infrastructure
{
    public interface IUserStoreAPIService
    {
        public Task<ValidateUserAccessResponse> Validate(string oidcSub, string entraObjectId, string email);
    }
}