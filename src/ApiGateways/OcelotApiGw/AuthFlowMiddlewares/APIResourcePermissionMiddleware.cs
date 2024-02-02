using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Authorization;
using Ocelot.Configuration;
using Ocelot.Middleware;
using OcelotApiGw.Contracts.Infrastructure;

namespace OcelotApiGw.AuthFlowMiddlewares
{
    public class APIResourcePermissionMiddleware
    {
        private readonly ILogger<APIResourcePermissionMiddleware> _logger;
        private readonly IUserStoreAPIService _userStoreAPIService;

        public APIResourcePermissionMiddleware(ILogger<APIResourcePermissionMiddleware> logger, IUserStoreAPIService userStoreAPIService)
        {
            _logger = logger;
            _userStoreAPIService = userStoreAPIService;
        }

        public async Task FetchUserPermissionsForAPI(HttpContext context)
        {
            _logger.LogInformation("Fetching user permissions for API in APIResourcePermissionMiddleware middleware.");

            if (context.Items.TryGetValue("DownstreamRoute", out var downstreamRouteObj))
            {
                var downstreamRoute = downstreamRouteObj as DownstreamRoute;
                var downstreamRouteTemplate = downstreamRoute?.DownstreamPathTemplate.Value;
                _logger.LogInformation($"Downstream route path is : {downstreamRouteTemplate}");

                if (downstreamRouteTemplate == null)
                {
                    _logger.LogError("Downstream route path is null, indicating a potential issue with the downstream route.");
                    context.Items.SetError(new UnauthorizedError("Downstream route path is null, indicating a potential issue with the downstream route."));
                    return;
                }


            }

            //await next.Invoke();
            return;
        }
    }
}