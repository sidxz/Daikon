using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace CQRS.Core.Extensions
{
    public static class HeaderLogExtensions
    {
        // Credentials must never reach logs; everything else is logged as sent
        private static readonly HashSet<string> CredentialHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization", "Proxy-Authorization", "Cookie"
        };

        /// <summary>The header's value for logging, with credentials replaced by "[redacted]".</summary>
        public static string LoggableValue(this KeyValuePair<string, StringValues> header) =>
            CredentialHeaders.Contains(header.Key) ? "[redacted]" : header.Value.ToString();
    }
}
