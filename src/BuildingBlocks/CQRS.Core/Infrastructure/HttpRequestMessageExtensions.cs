using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CQRS.Core.Infrastructure
{
    public static class HttpRequestMessageExtensions
    {
        // Only the caller's token and trace id travel between services; the receiving service resolves the user from the token
        private static readonly string[] PropagatedHeaders = { "Authorization", "trace-id" };

        public static void AddHeaders(this HttpRequestMessage request, IDictionary<string, string> headers)
        {
            foreach (var name in PropagatedHeaders)
            {
                if (headers.TryGetValue(name, out var value))
                {
                    request.Headers.TryAddWithoutValidation(name, value);
                }
            }
        }

        public static void AddHeaders(this HttpRequestMessage request, IHeaderDictionary headers)
        {
            foreach (var name in PropagatedHeaders)
            {
                if (headers.TryGetValue(name, out var value))
                {
                    request.Headers.TryAddWithoutValidation(name, value.ToString());
                }
            }
        }
    }

}