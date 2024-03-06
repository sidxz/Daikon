
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;

namespace UserStore.Application.Features.Queries.AppVars.GetAppVars
{
    public class GetAppVarsHandler : IRequestHandler<GetAppVarsQuery, GetAppVarsDTO>
    {
        private readonly ILogger<GetAppVarsHandler> _logger;
        private readonly IAppOrgRepository _appOrgRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppUserRepository _appUserRepository;

        public GetAppVarsHandler(ILogger<GetAppVarsHandler> logger, IAppOrgRepository appOrgRepository, IAppRoleRepository appRoleRepository, IAppUserRepository appUserRepository)
        {
            _logger = logger;
            _appOrgRepository = appOrgRepository;
            _appRoleRepository = appRoleRepository;
            _appUserRepository = appUserRepository;
        }

        public async Task<GetAppVarsDTO> Handle(GetAppVarsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var appOrgs = await _appOrgRepository.GetOrgsList();
                var appRoles = await _appRoleRepository.GetRolesList();
                var appUsers = await _appUserRepository.GetUsersList();

                // Consolidate dictionary creation
                var appOrgsDict = appOrgs.ToDictionary(x => x.Id, x => new { x.Name, x.Alias });
                var appRolesDict = appRoles.ToDictionary(x => x.Id, x => x.Name);
                var appUsersDict = appUsers.ToDictionary(x => x.Id, x => new { x.Email, FullName = x.FirstName + " " + x.LastName });

                var dto = new GetAppVarsDTO
                {
                    Orgs = appOrgsDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Name),
                    OrgsAlias = appOrgsDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Alias),
                    Roles = appRolesDict,
                    UserEmails = appUsersDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Email),
                    UserNames = appUsersDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.FullName)
                };

                if (request.UserId != null && request.UserId != Guid.Empty)
                {
                    var user = await _appUserRepository.GetUserById(request.UserId);
                    if (user != null)
                    {
                        dto.UsersRoles = new Dictionary<Guid, string>();
                        user.AppRoleIds.ForEach(roleId => dto.UsersRoles.Add(roleId, appRolesDict[roleId]));
                        dto.UsersOrg = new Tuple<Guid, string>(user.AppOrgId, appOrgsDict[user.AppOrgId].Name);
                    }
                }

                return dto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to get app vars");
                throw; // Rethrow the original exception to preserve its stack trace
            }
        }

        
    }
}
