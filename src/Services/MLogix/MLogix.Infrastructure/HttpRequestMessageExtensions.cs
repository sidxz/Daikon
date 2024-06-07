using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MLogix.Infrastructure
{
    public static class HttpRequestMessageExtensions
    {
        public static void AddHeaders(this HttpRequestMessage request, IDictionary<string, string> headers)
        {
           
            if (headers.TryGetValue("Authorization", out var authorization))
            {
                request.Headers.Add("Authorization", authorization);
            }
            if (headers.TryGetValue("AppUser-Id", out var appuserId))
            {
                request.Headers.Add("AppUser-Id", appuserId);
            }
            if (headers.TryGetValue("AppUser-Email", out var appuserEmail))
            {
                request.Headers.Add("AppUser-Email", appuserEmail);
            }
            if (headers.TryGetValue("AppUser-FullName", out var appuserFullname))
            {
                request.Headers.Add("AppUser-FullName", appuserFullname);
            }
            if (headers.TryGetValue("AppOrg-Id", out var apporgId))
            {
                request.Headers.Add("AppOrg-Id", apporgId);
            }
            if (headers.TryGetValue("AppRole-Ids", out var approleIds))
            {
                request.Headers.Add("AppRole-Ids", approleIds);
            }
        }
    }

    public static class HttpContextExtensions
    {
        public static IDictionary<string, string> GetSpecificHeaders(this HttpContext context, params string[] headerNames)
        {
            return context.Request.Headers
                .Where(h => headerNames.Contains(h.Key.ToLower()))
                .ToDictionary(h => h.Key, h => h.Value.ToString());
        }
    }


}