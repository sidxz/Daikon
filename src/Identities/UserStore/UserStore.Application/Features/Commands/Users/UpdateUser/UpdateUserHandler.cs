using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Users.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, AppUser>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppOrgRepository _appOrgRepository;

        public UpdateUserHandler(IMapper mapper, ILogger<UpdateUserHandler> logger, IAppUserRepository appUserRepository, IAppRoleRepository appRoleRepository, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _appOrgRepository = appOrgRepository;
        }

        public async Task<AppUser> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {


            var user = await _appUserRepository.GetUserById(request.Id);

            if (user == null)
            {
                throw new ResourceNotFoundException(nameof(AppUser), request.Id);
            }

            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.OIDCSub) && string.IsNullOrEmpty(request.EntraObjectId))
            {
                throw new ArgumentNullException("One of Email, OIDCSub, or EntraObjectId must be provided.");
            }

            // Check if email has changed, and if so, check if it is already in use
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingUser = await _appUserRepository.GetUserByEmail(request.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"Email {request.Email} is already in use.");
                }
            }

            // Check if OIDCSub has changed, and if so, check if it is already in use
            if (!string.IsNullOrEmpty(request.OIDCSub) && request.OIDCSub != user.OIDCSub)
            {
                var existingUser = await _appUserRepository.GetUserByOIDCSub(request.OIDCSub);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"OIDCSub {request.OIDCSub} is already in use.");
                }
            }

            // Check if EntraObjectId has changed, and if so, check if it is already in use
            if (!string.IsNullOrEmpty(request.EntraObjectId) && request.EntraObjectId != user.EntraObjectId)
            {
                var existingUser = await _appUserRepository.GetUserByEntraObjectId(request.EntraObjectId);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"EntraObjectId {request.EntraObjectId} is already in use.");
                }
            }

            // check if provided org exists
            if (request.AppOrgId != user.AppOrgId)
            {
                await CheckOrgExists(request.AppOrgId);
            }

            // if roles are provided, check if they exist
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                await CheckRolesExist(request.RoleIds);
            }

            _mapper.Map(request, user);

            try
            {
                await _appUserRepository.UpdateUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user.");
                throw new InvalidOperationException("An error occurred while updating user.", ex);
            }

            return user;
        }


        private async Task CheckOrgExists(Guid orgId)
        {
            var org = await _appOrgRepository.GetOrgById(orgId);
            if (org == null)
            {
                throw new InvalidOperationException("Org does not exist");
            }
        }

        private async Task CheckRolesExist(IEnumerable<Guid> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                var role = await _appRoleRepository.GetRoleById(roleId);
                if (role == null)
                {
                    throw new InvalidOperationException($"Role does not exist: {roleId}");
                }
            }
        }
    }


}
