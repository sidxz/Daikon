using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Users.OAuth2Connect
{
    /// <summary>
    /// Handles OAuth2 connection requests for first time sign in
    /// The user must exist in the database already
    /// Depending on the request, the user's EntraObjectId and/or OIDCSub will be updated
    /// If duplicate EntraObjectId or OIDCSub is detected, i.e. another user already has the same EntraObjectId or OIDCSub, the request will be rejected
    /// </summary>
    public class Oauth2ConnectHandler : IRequestHandler<Oauth2ConnectCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<Oauth2ConnectHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;

        public Oauth2ConnectHandler(IMapper mapper, ILogger<Oauth2ConnectHandler> logger, IAppUserRepository appUserRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
        }

        public async Task<Unit> Handle(Oauth2ConnectCommand request, CancellationToken cancellationToken)
        {
            ValidateEmail(request.Email);
            var user = await GetUserByEmail(request.Email);

            await HandleEntraObjectId(request, user);
            await HandleOIDCSub(request, user);

            await _appUserRepository.UpdateUser(user);

            return Unit.Value;
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                _logger.LogWarning("Invalid email address: {Email}", email);
                throw new InvalidOperationException("Email must be a valid email address");
            }
        }

        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await _appUserRepository.GetUserByEmail(email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", email);
                throw new InvalidOperationException("User not found");
            }

            return user;
        }

        /// <summary>
        /// Handles the assignment of EntraObjectId to the user if provided and not already set.
        /// </summary>
        /// <param name="request">The OAuth2 connect command containing the EntraObjectId.</param>
        /// <param name="user">The user to update.</param>
        private async Task HandleEntraObjectId(Oauth2ConnectCommand request, AppUser user)
        {
            if (request.EntraObjectId != Guid.Empty)
            {
                await UpdateUserAttribute(
                    user.EntraObjectId,
                    request.EntraObjectId,
                    user,
                    "EntraObjectId",
                    async () => await _appUserRepository.GetUserByEntraObjectId(request.EntraObjectId));
            }
        }

        /// <summary>
        /// Handles the assignment of OIDCSub to the user if provided and not already set.
        /// </summary>
        /// <param name="request">The OAuth2 connect command containing the OIDCSub.</param>
        /// <param name="user">The user to update.</param>

        private async Task HandleOIDCSub(Oauth2ConnectCommand request, AppUser user)
        {
            if (!string.IsNullOrEmpty(request.OIDCSub))
            {
                await UpdateUserAttribute(
                    user.OIDCSub,
                    request.OIDCSub,
                    user,
                    "OIDCSub",
                    async () => await _appUserRepository.GetUserByOIDCSub(request.OIDCSub));
            }
        }

        /// <summary>
        /// Generic method to update a user attribute if not already set and ensure its uniqueness across users.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to update.</typeparam>
        /// <param name="currentUserAttributeValue">The current value of the user's attribute.</param>
        /// <param name="newUserAttributeValue">The new value for the attribute.</param>
        /// <param name="user">The user to update.</param>
        /// <param name="attributeName">The name of the attribute to update.</param>
        /// <param name="getUserByAttribute">A function to retrieve a user by the attribute's value to ensure uniqueness.</param>

        private async Task UpdateUserAttribute<T>(T currentUserAttributeValue, T newUserAttributeValue, AppUser user, string attributeName, Func<Task<AppUser>> getUserByAttribute)
        {
            if (!EqualityComparer<T>.Default.Equals(currentUserAttributeValue, default(T)))
            {
                _logger.LogWarning($"User already has a {attributeName}: {currentUserAttributeValue}");
                throw new InvalidOperationException($"User already has a {attributeName}");
            }

            var existingUser = await getUserByAttribute();
            if (existingUser != null)
            {
                _logger.LogWarning($"{attributeName} already exists: {newUserAttributeValue}");
                throw new InvalidOperationException($"{attributeName} already exists");
            }

            typeof(AppUser).GetProperty(attributeName).SetValue(user, newUserAttributeValue);
        }
    }
}
