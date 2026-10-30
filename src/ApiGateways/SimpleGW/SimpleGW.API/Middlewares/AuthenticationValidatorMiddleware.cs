using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SimpleGW.API.Middlewares
{
    public class AuthenticationValidatorMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<AuthenticationValidatorMiddleware> _logger;
        public AuthenticationValidatorMiddleware(RequestDelegate next, ILogger<AuthenticationValidatorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // var jsonPrint = System.Text.Json.JsonSerializer.Serialize(context.User.Identity);
            // _logger.LogInformation("AuthenticationValidatorMiddleware : JSON");
            // _logger.LogInformation(jsonPrint);

            // Your auth validation logic here
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var emailClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                //_logger.LogInformation("AuthenticationValidatorMiddleware : User is authenticated SSO Email Claim : {email}", emailClaim);
                await _next(context); // Continue to next middleware
            }
            else
            {
                // 401 with WWW-Authenticate: Bearer (required by RFC 6750), including why the token was rejected
                _logger.LogWarning("AuthenticationValidatorMiddleware : User is not authenticated");
                await context.ChallengeAsync();
                return;
            }
        }
    }
}