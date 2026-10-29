using System;
using System.Security.Claims;

namespace CQRS.Core.Extensions
{
    /// <summary>
    /// Claims added to HttpContext.User after the bearer token's owner is resolved to a DAIKON user
    /// (Daikon.ApiHost AppUserMiddleware). Read identity from these, never from request headers.
    /// </summary>
    public static class DaikonClaims
    {
        public const string Prefix = "daikon:";
        public const string AppUserId = "daikon:app_user_id";
        public const string AppOrgId = "daikon:app_org_id";
        public const string FullName = "daikon:full_name";

        public static Guid GetAppUserId(this ClaimsPrincipal? user) => GetGuid(user, AppUserId);

        public static Guid GetAppOrgId(this ClaimsPrincipal? user) => GetGuid(user, AppOrgId);

        public static string? GetFullName(this ClaimsPrincipal? user) => user?.FindFirst(FullName)?.Value;

        private static Guid GetGuid(ClaimsPrincipal? user, string claimType) =>
            Guid.TryParse(user?.FindFirst(claimType)?.Value, out var id) ? id : Guid.Empty;
    }
}
