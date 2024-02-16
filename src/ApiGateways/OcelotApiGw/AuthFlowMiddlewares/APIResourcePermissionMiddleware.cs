
using Ocelot.Authorization;
using Ocelot.Configuration;
using Ocelot.Infrastructure;
using Ocelot.Middleware;
using Ocelot.Responses;
using OcelotApiGw.Contracts.Infrastructure;

namespace OcelotApiGw.AuthFlowMiddlewares
{
    public class APIResourcePermissionMiddleware
    {
        private readonly ILogger<APIResourcePermissionMiddleware> _logger;
        private readonly IUserStoreAPIService _userStoreAPIService;
        private readonly IPlaceholders _placeholders;
        public APIResourcePermissionMiddleware(ILogger<APIResourcePermissionMiddleware> logger, IUserStoreAPIService userStoreAPIService, IPlaceholders placeholders)
        {
            _logger = logger;
            _userStoreAPIService = userStoreAPIService;
            _placeholders = placeholders;
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

                // find out the AppUserId from the context Items
                var appUserId = context.Items["AppUserId"] as string;
                _logger.LogInformation($"AppUserId is : {appUserId}");
                if (appUserId == null)
                {
                    _logger.LogError("AppUserId is null, indicating a potential issue with the user context.");
                    context.Items.SetError(new UnauthorizedError("AppUserId is null, indicating a potential issue with the user context."));
                    return;
                }
                try
                {
                    var resolvePermissionsResponse = await _userStoreAPIService.ResolvePermission(appUserId: appUserId, method: context.Request.Method, endpoint: downstreamRouteTemplate);
                    _logger.LogInformation($"User permissions for API are : {resolvePermissionsResponse.AccessLevel}");
                    _placeholders.Add("{AppUserAccessLevel}", () => new OkResponse<string>(resolvePermissionsResponse.AccessLevel));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching user permissions for API in APIResourcePermissionMiddleware middleware.");
                    context.Items.SetError(new UnauthorizedError("Error occurred while fetching user permissions for API in APIResourcePermissionMiddleware middleware."));
                    return;
                }

                // ALL SUCCESS
                return;
            }


            // Fail Safe to prevent unauthorized access
            context.Items.SetError(new UnauthorizedError("Downstream route path is null, indicating a potential issue with the downstream route."));
            return;
        }
    }
}