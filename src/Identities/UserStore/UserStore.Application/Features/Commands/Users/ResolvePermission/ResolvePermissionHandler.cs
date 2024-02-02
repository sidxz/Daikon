
using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;

namespace UserStore.Application.Features.Commands.Users.ResolvePermission
{
    public class ResolvePermissionHandler : IRequestHandler<ResolvePermissionQuery, ResolvePermissionResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ResolvePermissionHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppOrgRepository _appOrgRepository;
        private readonly IAPIResourceRepository _apiResourceRepository;

        public ResolvePermissionHandler(IMapper mapper, ILogger<ResolvePermissionHandler> logger, IAppUserRepository appUserRepository, IAppRoleRepository appRoleRepository, IAppOrgRepository appOrgRepository, IAPIResourceRepository apiResourceRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _appOrgRepository = appOrgRepository;
            _apiResourceRepository = apiResourceRepository;
        }
        public async Task<ResolvePermissionResponse> Handle(ResolvePermissionQuery request, CancellationToken cancellationToken)
        {
            var user = await _appUserRepository.GetUserById(request.UserId);
            if (user == null)
            {
                throw new ResourceNotFoundException(nameof(ResolvePermissionResponse), $"User with ID {request.UserId} not found");
            }

            if (user.AppRoleIds == null || !user.AppRoleIds.Any())
            {
                throw new ResourceNotFoundException(nameof(ResolvePermissionResponse), $"User with ID {request.UserId} has no roles");
            }

            var usersRoles = await _appRoleRepository.GetRolesByIds(user.AppRoleIds);
            // Log user roles
            _logger.LogInformation($"User {user.Email} has roles: {string.Join(", ", usersRoles.Select(r => r.Name))}");

            var apiResource = await _apiResourceRepository.GetAPIResourceByEndPoint(request.Method, request.Endpoint);
            if (apiResource == null || apiResource.AttachedAppRoles == null)
            {
                throw new ResourceNotFoundException(nameof(ResolvePermissionResponse), $"API Resource for endpoint {request.Endpoint} not found");
            }

            var apiRoles = await _appRoleRepository.GetRolesByIds(apiResource.AttachedAppRoles);
            // Log API roles
            _logger.LogInformation($"API Resource {apiResource.Name} has roles: {string.Join(", ", apiRoles.Select(r => r.Name))}");

            // find which roles are common between user and API Resource by role id
            //var commonRoles = usersRoles.Where(ur => apiRoles.Any(ar => ar.Id == ur.Id)).ToList();
            var commonRoles = apiRoles.Where(ar => usersRoles.Any(ur => ur.Id == ar.Id)).ToList();


            // Log common roles
            _logger.LogInformation($"User {user.Email} has common roles with API Resource {apiResource.Name}: {string.Join(", ", commonRoles.Select(r => r.Name))}");

            var resolvePermissionResponse = new ResolvePermissionResponse
            {
                AccessLevel = "000",
                AccessLevelDescriptor = "SELF_ORG_ALL"
            };

            if (!commonRoles.Any())
            {
                return resolvePermissionResponse;
            }

            int selfAccessLevel = 0, organizationAccessLevel = 0, allAccessLevel = 0;
            foreach (var role in commonRoles)
            {
                // log role access levels
                _logger.LogInformation($"Role {role.Name} has access levels: SELF: {role.SelfAccessLevel}, ORG: {role.OrganizationAccessLevel}, ALL: {role.AllAccessLevel}");
                selfAccessLevel = Math.Max(selfAccessLevel, role.SelfAccessLevel);
                organizationAccessLevel = Math.Max(organizationAccessLevel, role.OrganizationAccessLevel);
                allAccessLevel = Math.Max(allAccessLevel, role.AllAccessLevel);

                // log max access levels
                _logger.LogInformation($"Max access levels: SELF: {selfAccessLevel}, ORG: {organizationAccessLevel}, ALL: {allAccessLevel}");
            }

            resolvePermissionResponse.AccessLevel = $"{selfAccessLevel}{organizationAccessLevel}{allAccessLevel}";

            return resolvePermissionResponse;
        }

    }
}