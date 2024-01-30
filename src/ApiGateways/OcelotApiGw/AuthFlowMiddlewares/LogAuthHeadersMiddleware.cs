using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OcelotApiGw.AuthFlowMiddlewares
{
    public class LogAuthHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogAuthHeadersMiddleware> _logger;

        public LogAuthHeadersMiddleware(RequestDelegate next, ILogger<LogAuthHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {   
            _logger.LogInformation(" ***************** Middleware - Headers:");
            foreach (var claim in context.User.Claims)
            {
                _logger.LogInformation($"Middleware - Claim type: {claim.Type}, value: {claim.Value}");
            }

            await _next(context);
        }
    }


}