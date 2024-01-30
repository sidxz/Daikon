using AutoMapper;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Users.AddUser
{
    public class AddUserHandler : IRequestHandler<AddUserCommand, AppUser>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly IAppOrgRepository _appOrgRepository;

        public AddUserHandler(IMapper mapper, ILogger<AddUserHandler> logger, IAppUserRepository appUserRepository, IAppRoleRepository appRoleRepository, IAppOrgRepository appOrgRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _appRoleRepository = appRoleRepository;
            _appOrgRepository = appOrgRepository;
        }

        public async Task<AppUser> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            await CheckForExistingUserAttributes(request);
            await CheckOrgExists(request.AppOrgId);

            // check only if roles are provided
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                await CheckRolesExist(request.RoleIds);
            }
            

            var newUser = _mapper.Map<AppUser>(request);

            InitializeNewUserProperties(newUser);

            try
            {
                await _appUserRepository.AddUser(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user.");
                throw new InvalidOperationException("An error occurred while adding user.", ex);
            }

            return newUser;
        }

        private void ValidateRequest(AddUserCommand request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.OIDCSub) && string.IsNullOrEmpty(request.EntraObjectId))
            {
                throw new ArgumentNullException("One of Email, OIDCSub, or EntraObjectId must be provided.");
            }
        }

        private async Task CheckForExistingUserAttributes(AddUserCommand request)
        {
            if (!string.IsNullOrEmpty(request.Email))
            {
                await CheckAttributeUniqueness(() => _appUserRepository.GetUserByEmail(request.Email), "User email already exists");
            }

            if (!string.IsNullOrEmpty(request.OIDCSub))
            {
                await CheckAttributeUniqueness(() => _appUserRepository.GetUserByOIDCSub(request.OIDCSub), "User OIDCSub already exists");
            }

            if (!string.IsNullOrEmpty(request.EntraObjectId))
            {
                await CheckAttributeUniqueness(() => _appUserRepository.GetUserByEntraObjectId(request.EntraObjectId), "User EntraObjectId already exists");
            }
        }

        private async Task CheckAttributeUniqueness(Func<Task<AppUser>> getUserMethod, string errorMessage)
        {
            var existingUser = await getUserMethod();
            if (existingUser != null)
            {
                throw new DuplicateEntityRequestException(nameof(AddUserCommand), errorMessage);
            }
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

        private void InitializeNewUserProperties(AppUser newUser)
        {
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.CreatedBy = "System";
            newUser.IsUserLocked = false;
            newUser.IsUserArchived = false;
            newUser.IsOIDCConnected = false;
            newUser.Email = newUser.Email;
            newUser.NormalizedEmail = newUser.Email.ToUpperInvariant();
        }
    }
}
