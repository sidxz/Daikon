
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Application.Contracts.Persistence;
using UserStore.Application.Features.Commands.Users.OAuth2Connect;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Commands.Users.ValidateUserAccess
{
    public class ValidateUserAccessHandler : IRequestHandler<ValidateUserAccessCommand, ValidateUserAccessResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ValidateUserAccessHandler> _logger;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IMediator _mediator;

        public ValidateUserAccessHandler(IMapper mapper, ILogger<ValidateUserAccessHandler> logger, IAppUserRepository appUserRepository, IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _appUserRepository = appUserRepository;
            _mediator = mediator;
        }

        public async Task<ValidateUserAccessResponse> Handle(ValidateUserAccessCommand request, CancellationToken cancellationToken)
        {
            // Initialize the response object with default invalid state
            var response = new ValidateUserAccessResponse
            {
                IsValid = false
            };

            // Validate input parameters
            ValidateRequest(request);

            // Attempt to validate user access by EntraObjectId, OIDCSub, and Email, in that order.
            _ = await ValidateAccessByEntraObjectId(request, response) ||
            await ValidateAccessByOIDCSub(request, response) ||
            await ValidateAccessByEmailAndUpdateOAuth2Attributes(request, response);

            return response;
        }

        private void ValidateRequest(ValidateUserAccessCommand request)
        {
            // Check if all identifiers are missing
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.OIDCSub) && string.IsNullOrEmpty(request.EntraObjectId))
            {
                _logger.LogWarning("Either Email, OIDCSub, or EntraObjectId must be provided.");
                throw new InvalidOperationException("Either Email, OIDCSub, or EntraObjectId must be provided.");
            }
        }

        private async Task<bool> ValidateAccessByEntraObjectId(ValidateUserAccessCommand request, ValidateUserAccessResponse response)
        {
            if (!string.IsNullOrEmpty(request.EntraObjectId))
            {
                var user = await _appUserRepository.GetUserByEntraObjectId(request.EntraObjectId);
                // Match with email if provided
                if (user != null && !string.IsNullOrEmpty(request.Email) && user.NormalizedEmail != request.Email.ToUpperInvariant())
                {
                    _logger.LogWarning("Email for EntraObjectId {EntraObjectId} - {AppUserEmail} does not match with Oauth2 email {Email}", request.EntraObjectId, user.NormalizedEmail, request.Email.ToUpperInvariant());
                    return false;
                }
                return user != null && UpdateResponseWithUserDetails(user, response);
            }
            return false;
        }

        private async Task<bool> ValidateAccessByOIDCSub(ValidateUserAccessCommand request, ValidateUserAccessResponse response)
        {
            if (!string.IsNullOrEmpty(request.OIDCSub))
            {
                var user = await _appUserRepository.GetUserByOIDCSub(request.OIDCSub);
                // Match with email if provided
                if (user != null && !string.IsNullOrEmpty(request.Email) && user.NormalizedEmail != request.Email.ToUpperInvariant())
                {
                    _logger.LogWarning("Email for OIDCSub {OIDCSub} - {AppUserEmail} does not match with Oauth2 email {Email}", request.OIDCSub, user.NormalizedEmail, request.Email.ToUpperInvariant());
                    return false;
                }
                return user != null && UpdateResponseWithUserDetails(user, response);
            }
            return false;
        }

        private async Task<bool> ValidateAccessByEmailAndUpdateOAuth2Attributes(ValidateUserAccessCommand request, ValidateUserAccessResponse response)
        {
            if (!string.IsNullOrEmpty(request.Email))
            {
                var user = await _appUserRepository.GetUserByEmail(request.Email);
                if (user != null)
                {
                    UpdateResponseWithUserDetails(user, response);
                    await UpdateOAuth2AttributesForUser(user, request);
                    return true;
                }
            }
            return false;
        }

        private bool UpdateResponseWithUserDetails(AppUser user, ValidateUserAccessResponse response)
        {
            _logger.LogInformation("Authentication [SUCCESS]: User {AppUserEmail} validated successfully at {Timestamp}", user.NormalizedEmail, DateTime.Now);
            response.IsValid = true;
            response.AppUserId = user.Id;
            response.Email = user.Email;
            response.NormalizedEmail = user.NormalizedEmail;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.AppOrgId = user.AppOrgId;
            response.AppRoleIds = user.AppRoleIds;
            response.IsSystemAccount = user.IsSystemAccount;
            return true;
        }

        private async Task UpdateOAuth2AttributesForUser(AppUser user, ValidateUserAccessCommand request)
        {
            var oauth2ConnectCommand = new OAuth2ConnectCommand
            {
                Email = user.Email,
                EntraObjectId = request.EntraObjectId,
                OIDCSub = request.OIDCSub
            };

            try
            {
                await _mediator.Send(oauth2ConnectCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating OAuth2 attributes for user with email {Email}", user.Email);
                throw;
            }
        }
    }
}
