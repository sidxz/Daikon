using System.Security.Claims;
using CQRS.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace Daikon.ApiHost;

public record AppUserIdentity(Guid AppUserId, Guid AppOrgId, string FullName);

public interface IAppUserResolver
{
    // The DAIKON user that owns the validated token on context.User, or null if unknown or locked.
    Task<AppUserIdentity?> Resolve(HttpContext context);
}

// Runs after token validation: maps the token's owner to the DAIKON user and adds DaikonClaims to context.User.
// Identity comes only from the signed token; AppUser-* request headers are never read.
public class AppUserMiddleware
{
    // Each service and UserStore both cache, so a lock or org change reaches every service within 2 x 5 = 10 min
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    // "Not a DAIKON user" is remembered briefly too, so repeated requests with such a token don't each hit UserStore
    private static readonly TimeSpan UnknownUserCacheDuration = TimeSpan.FromSeconds(30);

    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AppUserMiddleware> _logger;

    public AppUserMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<AppUserMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            // Anonymous endpoints (health) carry on; authorization rejects everything else
            await _next(context);
            return;
        }

        var subject = context.User.GetObjectId() ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(subject))
        {
            await Reject(context, "Token has no subject");
            return;
        }

        var cacheKey = $"AppUser_{subject}";
        if (!_cache.TryGetValue(cacheKey, out AppUserIdentity? user))
        {
            user = await context.RequestServices.GetRequiredService<IAppUserResolver>().Resolve(context);
            _cache.Set(cacheKey, user, user is null ? UnknownUserCacheDuration : CacheDuration);
        }
        if (user is null)
        {
            await Reject(context, "User validation failed");
            return;
        }

        // Only this middleware may assert DAIKON identity: drop any daikon:* claims that arrived inside the token
        foreach (var identity in context.User.Identities)
        {
            foreach (var claim in identity.Claims.Where(c => c.Type.StartsWith(DaikonClaims.Prefix)).ToList())
            {
                identity.TryRemoveClaim(claim);
            }
        }

        context.User.AddIdentity(new ClaimsIdentity(new[]
        {
            new Claim(DaikonClaims.AppUserId, user.AppUserId.ToString()),
            new Claim(DaikonClaims.AppOrgId, user.AppOrgId.ToString()),
            new Claim(DaikonClaims.FullName, user.FullName),
        }));

        await _next(context);
    }

    private async Task Reject(HttpContext context, string message)
    {
        _logger.LogWarning("AppUserMiddleware: {Message}", message);
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\""; // required on 401 (RFC 6750)
        await context.Response.WriteAsJsonAsync(new { message });
    }
}
