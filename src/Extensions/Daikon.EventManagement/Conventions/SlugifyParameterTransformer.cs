using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Daikon.EventManagement.Conventions
{
    /*
     * SlugifyParameterTransformer
     * ---------------------------
     * Transforms route tokens (e.g., controller/action names) from PascalCase to kebab-case
     * for cleaner URLs like /my-action instead of /MyAction.
     *
     * Example:
     *   "MyActionMethod" => "my-action-method"
     *   "GetHTTPData"    => "get-http-data"
     */
    public sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            var str = value.ToString();
            if (string.IsNullOrEmpty(str)) return null;

            // Insert hyphen between lowercase-to-uppercase and acronym boundaries
            var slugified = Regex.Replace(str, 
                "([a-z0-9])([A-Z])", "$1-$2")             // Normal PascalCase
                               .Replace("--", "-")           // Avoid double hyphens
                               .ToLowerInvariant();

            return slugified;
        }
    }
}
