using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleGW.Contracts.Infrastructure;

namespace SimpleGW.API.Middlewares
{
    public class UserProfileEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserProfileEnricherMiddleware> _logger;

        public UserProfileEnricherMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<UserProfileEnricherMiddleware> logger)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("UserProfileEnricherMiddleware : Enriching user profile");
            try
            {
                var entraObjectIdentifierClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
                var emailClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                // 1. Entra Object Identifier Claim
                if (entraObjectIdentifierClaim?.Value != null)
                {
                    // entraObjectIdentifierClaim hints that the OAuth2 provider is Microsoft Entra ID
                    _logger.LogInformation("User will be authorized with either entraObjectId or EmailClaim: {entraObjectId} or {email}",
                                        entraObjectIdentifierClaim.Value, emailClaim?.Value);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userStoreService = scope.ServiceProvider.GetRequiredService<IUserStoreAPIService>();
                        var validateUserAccessResponse = await userStoreService.Validate(oidcSub: null, entraObjectId: entraObjectIdentifierClaim.Value, email: emailClaim?.Value);
                        if (validateUserAccessResponse.IsValid)
                        {
                            _logger.LogInformation("UserProfileEnricherMiddleware : User is authorized");
                            context.Request.Headers.Append("AppUser-Id", validateUserAccessResponse?.AppUserId.ToString());
                            context.Request.Headers.Append("AppUser-Email", validateUserAccessResponse?.NormalizedEmail);
                            context.Request.Headers.Append("AppUser-FullName", validateUserAccessResponse?.FirstName + " " + validateUserAccessResponse?.LastName);
                            context.Request.Headers.Append("AppOrg-Id", validateUserAccessResponse?.AppOrgId.ToString());

                            // Convert List<Guid> to comma-separated string safely
                            if (validateUserAccessResponse.AppRoleIds != null && validateUserAccessResponse.AppRoleIds.Any())
                            {
                                var roleIdsString = string.Join(",", validateUserAccessResponse.AppRoleIds);
                                context.Request.Headers.Append("AppRole-Ids", roleIdsString);
                            }
                            else
                            {
                                context.Request.Headers.Append("AppRole-Ids", string.Empty);
                            }

                            await _next(context); // Continue to next middleware
                            return;
                        }
                        else
                        {
                            _logger.LogError("UserProfileEnricherMiddleware: User validation failed");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "User validation failed" }));
                            return; // Ensure no further code in this middleware is executed after this point
                        }
                    }
                }

                // 2. OIDC Sub Claim TODO: Add support for other OIDC providers


                // 3. No valid claims Fail the request with 401
                // If entraObjectIdentifierClaim is null or other conditions for authorization are not met
                _logger.LogError("UserProfileEnricherMiddleware: Missing or invalid claims");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Missing or invalid claims" }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserProfileEnricherMiddleware: An exception occurred while enriching user profile");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "An internal server error occurred while processing the request" }));
            }
        }


    }
}