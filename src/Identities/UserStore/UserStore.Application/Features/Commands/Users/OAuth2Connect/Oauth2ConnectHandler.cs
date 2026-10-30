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
    /// Depending on the request, the user's EntraObjectId and/or OIDCSub will be recorded
    /// If duplicate EntraObjectId or OIDCSub is detected, i.e. another user already has the same EntraObjectId or OIDCSub, the request will be rejected
    /// </summary>
    public class OAuth2ConnectHandler : IRequestHandler<OAuth2ConnectCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<OAuth2ConnectHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;

        public OAuth2ConnectHandler(IMapper mapper, ILogger<OAuth2ConnectHandler> logger, IAppUserRepository appUserRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
        }

        public async Task<Unit> Handle(OAuth2ConnectCommand request, CancellationToken cancellationToken)
        {
            ValidateEmail(request.Email);
            var user = await GetUserByEmail(request.Email);

            var entraObjectId = await IdentityToRecord(user, user.EntraObjectId, request.EntraObjectId, "EntraObjectId", _appUserRepository.GetUserByEntraObjectId);
            var oidcSub = await IdentityToRecord(user, user.OIDCSub, request.OIDCSub, "OIDCSub", _appUserRepository.GetUserByOIDCSub);
            if (entraObjectId is null && oidcSub is null)
            {
                return Unit.Value; // already linked to this identity
            }

            // One conditional update of the identity fields only. It matches nothing if the account was locked, linked to
            // another identity or deleted since it was read, and it never rewrites the rest of the record from that read.
            if (!await _appUserRepository.LinkIdentity(user.Id, entraObjectId, oidcSub))
            {
                _logger.LogWarning("Account {UserId} changed while linking: locked, linked elsewhere or deleted", user.Id);
                throw new AccountLinkingRefusedException("Account changed during sign-in");
            }

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
                throw new AccountLinkingRefusedException("User not found");
            }
            return user;
        }

        /// <summary>
        /// The identity value to record for this attribute, or null when there is nothing to record: not in the request,
        /// or the account already carries this value (a concurrent first sign-in got here first).
        /// Refuses when the account is linked to a different identity, or the identity belongs to another account.
        /// </summary>
        private async Task<string?> IdentityToRecord(AppUser user, string? current, string? requested, string attributeName, Func<string, Task<AppUser>> getUserByAttribute)
        {
            if (string.IsNullOrEmpty(requested))
            {
                return null;
            }
            if (!string.IsNullOrEmpty(current))
            {
                if (current == requested)
                {
                    return null;
                }
                _logger.LogWarning("User already has a {AttributeName}: {Current}", attributeName, current);
                throw new AccountLinkingRefusedException($"User already has a {attributeName}");
            }
            var existingUser = await getUserByAttribute(requested);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                _logger.LogWarning("{AttributeName} already exists: {Requested}", attributeName, requested);
                throw new AccountLinkingRefusedException($"{attributeName} already exists");
            }
            // New, or this same account linked concurrently and read before that write: record it (the update accepts an identical value)
            return requested;
        }
    }
}
