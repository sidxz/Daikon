using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SimpleGW.Contracts.Infrastructure;
using SimpleGW.DTOs;

namespace SimpleGW.API.Middlewares
{
    public class UserProfileEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserProfileEnricherMiddleware> _logger;
        private readonly IMemoryCache _cache;

        public UserProfileEnricherMiddleware(
            RequestDelegate next, IServiceProvider serviceProvider, ILogger<UserProfileEnricherMiddleware> logger, IMemoryCache cache)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var entraObjectIdentifierClaim = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
                var oidcSubClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var emailClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                if (entraObjectIdentifierClaim == null && oidcSubClaim == null)
                {
                    _logger.LogWarning("Missing required claims in the request.");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Unauthorized: Missing claims" }));
                    return;
                }

                string cacheKey = $"UserProfile_{entraObjectIdentifierClaim?.Value ?? oidcSubClaim?.Value}";

                if (!_cache.TryGetValue(cacheKey, out ValidateUserAccessResponse validateUserAccessResponse))
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userStoreService = scope.ServiceProvider.GetRequiredService<IUserStoreAPIService>();

                        // Depending on the available claim, call the Validate method
                        try
                        {
                            validateUserAccessResponse = await userStoreService.Validate(
                                oidcSub: oidcSubClaim?.Value,
                                entraObjectId: entraObjectIdentifierClaim?.Value,
                                email: emailClaim?.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred while calling the user store service.");
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Internal server error" }));
                            return;
                        }

                        // If the validation is successful, store it in the cache
                        if (validateUserAccessResponse.IsValid == true)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
                            };

                            _cache.Set(cacheKey, validateUserAccessResponse, cacheEntryOptions);
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "User validation failed" }));
                            return;
                        }
                    }
                }

                // Add user info to request headers
                context.Request.Headers.Append("AppUser-Id", validateUserAccessResponse?.AppUserId.ToString());
                context.Request.Headers.Append("AppUser-Email", validateUserAccessResponse?.NormalizedEmail);
                context.Request.Headers.Append("AppUser-FullName", $"{validateUserAccessResponse?.FirstName} {validateUserAccessResponse?.LastName}");
                context.Request.Headers.Append("AppOrg-Id", validateUserAccessResponse?.AppOrgId.ToString());

                if (validateUserAccessResponse?.AppRoleIds != null && validateUserAccessResponse.AppRoleIds.Count != 0)
                {
                    context.Request.Headers.Append("AppRole-Ids", string.Join(",", validateUserAccessResponse.AppRoleIds));
                }

                await _next(context); // Continue to next middleware

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing UserProfileEnricherMiddleware.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"An internal server error occurred.\"}");
                // No need to call the next middleware here as we're handling the error
            }
        }

    }
}