using Microsoft.AspNetCore.Http;

namespace Daikon.ApiHost;

// Security and no-cache headers on every response, at the gateway and in each service.
// Set by the app itself rather than trusted to whatever proxy or ingress hosts DAIKON, and in OnStarting
// so nothing written later (an error handler, or headers the gateway copies from a service) can override them.
public class SecurityHeadersMiddleware
{
    private static readonly string[] CachingHeaders =
    {
        "Expires", "ETag", "Last-Modified", "Age", "Surrogate-Control", "CDN-Cache-Control", "Cloudflare-CDN-Cache-Control"
    };

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            // Per-user data and auth errors: no browser, proxy or CDN may store them
            headers["Cache-Control"] = "no-store, max-age=0";
            headers["Pragma"] = "no-cache";

            // JSON API: nothing may be rendered, framed, sniffed, embedded or referred from it
            headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Cross-Origin-Resource-Policy"] = "same-origin";

            // Browsers ignore this over plain HTTP and enforce HTTPS once it arrives over TLS
            headers["Strict-Transport-Security"] = "max-age=31536000";

            // Kestrel's own Server header is switched off; this drops one copied from elsewhere
            headers.Remove("Server");

            // Nothing may re-enable caching: CDNs obey their own headers over Cache-Control, and validators or expiry
            // dates only make sense for stored responses
            foreach (var name in CachingHeaders)
            {
                headers.Remove(name);
            }
            return Task.CompletedTask;
        });

        return _next(context);
    }
}
