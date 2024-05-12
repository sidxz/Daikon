using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleGW.API.Middlewares
{
    public class MicroServiceRoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<MicroServiceRoutingMiddleware> _logger;
        private IConfiguration _configuration;

        public MicroServiceRoutingMiddleware(RequestDelegate next, ILogger<MicroServiceRoutingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var pathSegments = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (pathSegments != null && pathSegments.Length > 1)
                {
                    var microserviceName = pathSegments[1].ToLower(); // Convert to lowercase for case-insensitive comparison
                    //_logger.LogInformation($"Extracted microservice name: {microserviceName}");

                    var endPointRouting = _configuration.GetSection("EndPointRouting").Get<Dictionary<string, string>>();

                    if (endPointRouting != null && endPointRouting.Any())
                    {
                        var routes = endPointRouting.ToDictionary(k => k.Key.ToLower(), v => v.Value); // Convert keys to lowercase for case-insensitive matching

                        if (routes.TryGetValue(microserviceName, out var targetEndpoint))
                        {
                            //_logger.LogInformation($"Routing {context.Request.Path.Value} to {targetEndpoint}");
                            context.Items["Microservice"] = targetEndpoint; // Set the microservice endpoint
                        }
                        else
                        {
                            _logger.LogWarning($"No routing match found for {context.Request.Path.Value}. Could not find Microservice Endpoint.");
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\":\"Could not find Microservice Endpoint.\"}");
                            return; // Exit to prevent calling the next middleware
                        }
                    }
                    else
                    {
                        _logger.LogWarning("EndPointRouting configuration is missing or empty.");
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"message\":\"EndPointRouting configuration is missing or empty.\"}");
                        return; // Exit to prevent calling the next middleware
                    }
                }
                else
                {
                    _logger.LogWarning($"Request path {context.Request.Path.Value} is invalid or does not contain enough segments to determine the microservice.");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\":\"Invalid request path.\"}");
                    return; // Exit to prevent calling the next middleware
                }

                await _next(context); // Continue processing with the next middleware in the pipeline if all checks pass
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing MicroServiceRoutingMiddleware.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"An internal server error occurred.\"}");
                // No need to call the next middleware here as we're handling the error
            }
        }




    }
}