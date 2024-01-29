using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Authorization;
using Ocelot.Errors;
using Ocelot.Middleware;
using OcelotApiGw.HTTPResponses;

namespace OcelotApiGw.AuthFlowMiddlewares
{
    public class OAuth2UserAccessHandler
    {
        private readonly ILogger<OAuth2UserAccessHandler> _logger;

        public OAuth2UserAccessHandler(ILogger<OAuth2UserAccessHandler> logger)
        {
            _logger = logger;
        }
        public async Task ValidateUser(HttpContext context, Func<Task> next)
        {
            _logger.LogInformation(" ***************** Middleware - Headers:");

            foreach (var claim in context.User.Claims)
            {
                _logger.LogInformation($"Middleware - Claim type: {claim.Type}, value: {claim.Value}");
            }

            context.Items.SetError(new UnauthorizedError("your custom message"));

            //await ResponseUtil.WriteUnauthorizedResponseAsync(context, "Access denied by User Service");
            //throw new HttpRequestException("Access denied due to custom validation logic.");


            return;
            Console.WriteLine(" XXXXX Should have returned by now XXXXX");
            await next.Invoke();
        }
    }
}