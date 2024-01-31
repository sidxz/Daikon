using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ocelot.Authorization;
using Ocelot.Errors;
using Ocelot.Middleware;
using OcelotApiGw.Contracts.Infrastructure;

/*
    == Overview
    OAuth2UserAccessHandler is a middleware component in the Ocelot API Gateway designed to 
    validate user access based on OAuth2 claims. It interacts with an external User Store API 
    through the IUserStoreAPIService to confirm if a user is authorized.

    == Dependencies
    IUserStoreAPIService: The service responsible for validating the user's access by 
    communicating with an external User Store API.
*/
namespace OcelotApiGw.AuthFlowMiddlewares
{
    public class OAuth2UserAccessHandler
    {
        private readonly ILogger<OAuth2UserAccessHandler> _logger;
        private readonly IUserStoreAPIService _userStoreAPIService;

        public OAuth2UserAccessHandler(ILogger<OAuth2UserAccessHandler> logger, IUserStoreAPIService userStoreAPIService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userStoreAPIService = userStoreAPIService ?? throw new ArgumentNullException(nameof(userStoreAPIService));
        }

        public async Task ValidateUser(HttpContext context, Func<Task> next)
        {
            _logger.LogInformation("Validating user access in OAuth2UserAccessHandler middleware.");

            var entraObjectIdentifierClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            var emailClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (entraObjectIdentifierClaim?.Value != null)
            {
                // entraObjectIdentifierClaim hints that the OAuth2 provider is Microsoft Entra ID
                _logger.LogInformation("User will be validated with entraObjectId: {entraObjectId}", entraObjectIdentifierClaim.Value);
                var email = emailClaim?.Value;

                var validateUserAccessResponse = await _userStoreAPIService.Validate(oidcSub: null, entraObjectId: entraObjectIdentifierClaim.Value, email: email);
                if (validateUserAccessResponse == null)
                {
                    _logger.LogError("UserStoreAPI response is null, indicating a potential issue with the UserStore service.");
                    context.Items.SetError(new UnauthorizedError("UserStoreAPI response is null, indicating a potential issue with the UserStore service."));
                    return;
                }

                if (validateUserAccessResponse.IsValid)
                {
                    _logger.LogInformation("User access successfully validated for email using EntraId: {email}. Proceeding to next middleware.", validateUserAccessResponse.Email);
                    await next.Invoke();
                    return;
                }

                _logger.LogError("User access validation failed for entraObjectId: {entraObjectId}. The user might not be registered in the UserStore.", entraObjectIdentifierClaim.Value);
                context.Items.SetError(new UnauthorizedError("User access validation failed. The user might not be registered in the UserStore."));
                return;
            }

            // More OAuth2 providers can be added here

            _logger.LogError("User access validation failed. No More OAuth2 Providers are registered.");
            context.Items.SetError(new UnauthorizedError("User access validation failed. No More OAuth2 Providers are registered."));
            return;

        }
    }
}
