using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Authorization;
using Ocelot.Errors;
using Ocelot.Middleware;
using OcelotApiGw.Contracts.Infrastructure;


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
            _logger.LogInformation(" ***************** Middleware - OAuth2UserAccessHandler");

            var objectIdentifierClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
            var emailClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            // log objectIdentifierClaim and emailClaim
            _logger.LogInformation($"Middleware - Claim type: objectIdentifierClaim, value: {objectIdentifierClaim.Value}");
            _logger.LogInformation($"Middleware - Claim type: emailClaim, value: {emailClaim.Value}");

            if (objectIdentifierClaim.Value == null)
            {
                context.Items.SetError(new UnauthorizedError("User is not authenticated"));
                return;
            }

            var validateUserAccessResponse = await _userStoreAPIService.Validate(null, objectIdentifierClaim.Value, emailClaim.Value);
            if (validateUserAccessResponse == null)
            {
                context.Items.SetError(new UnauthorizedError("User is not authorized"));
                return;
            }
            // see if validateUserAccessResponse is valid is true then succeed
            if (validateUserAccessResponse.IsValid)
            {
                await next.Invoke();
                return;
            }

            context.Items.SetError(new UnauthorizedError("your custom message"));
            return;
        }
    }
}