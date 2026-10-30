using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Daikon.ApiHost;

// Default IAppUserResolver: asks UserStore who the token belongs to, sending that same token.
// UserStore itself registers an in-process resolver instead (calling itself would recurse).
public class UserStoreAppUserResolver : IAppUserResolver
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserStoreAppUserResolver> _logger;
    private readonly string _currentUserUrl;

    public UserStoreAppUserResolver(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<UserStoreAppUserResolver> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        var baseUrl = configuration["UserStoreAPI:Url"]
            ?? throw new InvalidOperationException("UserStoreAPI:Url configuration is missing.");
        _currentUserUrl = $"{baseUrl.TrimEnd('/')}/user/header";
    }

    public async Task<AppUserIdentity?> Resolve(HttpContext context)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _currentUserUrl);
        request.Headers.TryAddWithoutValidation("Authorization", context.Request.Headers.Authorization.ToString());

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(10); // a slow UserStore is an error here, not a 100 s stall per request
        using var response = await client.SendAsync(request, context.RequestAborted);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.NotFound)
        {
            // Usually "not a DAIKON user, or locked". If every user lands here, UserStore itself is rejecting the token:
            // its WWW-Authenticate says why (e.g. a different audience configured there).
            _logger.LogWarning("UserStore did not resolve the user: HTTP {Status}, WWW-Authenticate: {Challenge}",
                (int)response.StatusCode, response.Headers.WwwAuthenticate.ToString());
            return null;
        }
        response.EnsureSuccessStatusCode(); // UserStore failing is a 500, not a silent 401

        var user = await response.Content.ReadFromJsonAsync<CurrentUser>(context.RequestAborted);
        return user is null ? null : new AppUserIdentity(user.Id, user.AppOrgId, $"{user.FirstName} {user.LastName}");
    }

    private record CurrentUser(Guid Id, Guid AppOrgId, string? FirstName, string? LastName);
}
