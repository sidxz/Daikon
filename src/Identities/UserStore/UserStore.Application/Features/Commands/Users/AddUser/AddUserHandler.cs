
using AutoMapper;
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

        public AddUserHandler(IMapper mapper, ILogger<AddUserHandler> logger, IAppUserRepository appUserRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
        }

        public async Task<AppUser> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            // if both email and oidcsub are null, reject
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.OIDCSub))
            {
                _logger.LogWarning("Either Email or OIDCSub must be provided.");
                throw new InvalidOperationException("Either Email or OIDCSub must be provided.");
            }

            // check if user email exists (If it is set)
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingUser = await _appUserRepository.GetUserByEmail(request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User email already exists: {Email}", request.Email);
                    throw new InvalidOperationException("User email already exists");
                }
            }

            // check if user OIDCSub exists (If it is set)
            if (!string.IsNullOrEmpty(request.OIDCSub))
            {
                var existingUser = await _appUserRepository.GetUserByOIDCSub(request.OIDCSub);
                if (existingUser != null)
                {
                    _logger.LogWarning("User OIDCSub already exists: {OIDCSub}", request.OIDCSub);
                    throw new InvalidOperationException("User OIDCSub already exists");
                }
            }

            // check if org exists
            var org = await _appOrgRepository.GetOrgById(request.OrgId);
            if (org == null)
            {
                _logger.LogWarning("Org does not exist: {OrgId}", request.OrgId);
                throw new InvalidOperationException("Org does not exist");
            }

            // loop through roles and check if they exist
            foreach (var roleId in request.RoleIds)
            {
                var role = await _appRoleRepository.GetRoleById(roleId);
                if (role == null)
                {
                    _logger.LogWarning("Role does not exist: {RoleId}", roleId);
                    throw new InvalidOperationException("Role does not exist");
                }
            }

            var newUser = _mapper.Map<AppUser>(request);

            newUser.CreatedAt = DateTime.UtcNow;
            newUser.CreatedBy = "System";
            newUser.IsUserLocked = false;
            newUser.IsUserArchived = false;
            newUser.HasFirstLogin = false;
            newUser.NormalizedEmail = newUser.Email.ToUpper();
            try
            {
                await _appUserRepository.AddUser(newUser);
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user");
                throw;
            }
        }
    }
}