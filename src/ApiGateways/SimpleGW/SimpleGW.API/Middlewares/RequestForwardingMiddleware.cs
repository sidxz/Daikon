
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace SimpleGW.API.Middlewares
{
    public class RequestForwardingMiddleware
    {
        // Zero trust in both directions: only these headers cross the gateway. Everything else the client sends
        // (cookies, X-Forwarded-*, hop-by-hop, identity or trace headers) stays out of the services, and everything
        // else a service returns (Server, internal URLs, hop-by-hop, CORS/caching headers the gateway owns) stays in.
        private static readonly string[] ForwardedRequestHeaders = { "Authorization", "Accept", "x-api-version" };
        private static readonly string[] ReturnedResponseHeaders = { "WWW-Authenticate", "Retry-After", "api-supported-versions", "api-deprecated-versions" };
        private static readonly string[] ReturnedContentHeaders = { "Content-Type", "Content-Disposition", "Content-Language" };

        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RequestForwardingMiddleware> _logger;

        public RequestForwardingMiddleware(RequestDelegate next, IHttpClientFactory clientFactory, ILogger<RequestForwardingMiddleware> logger)
        {
            _next = next;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var microservice = context.Items["Microservice"] as string;
            if (string.IsNullOrWhiteSpace(microservice))
            {
                _logger.LogWarning("Microservice URL is not set in the context items. Aborting the request forwarding.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"Microservice endpoint is not configured.\"}");
                return; // Do not proceed with the request forwarding
            }

            var targetUrl = $"{microservice}/api{context.Request.Path.Value}{context.Request.QueryString}";
            //_logger.LogInformation($"Forwarding request to {targetUrl}");

            try
            {
                var client = _clientFactory.CreateClient("SimpleGWClient");
                var proxiedRequest = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);
                // Ensure our trace-id is forwarded
                var traceId = context.Items["TraceId"] as string
                              ?? Activity.Current?.TraceId.ToString()
                              ?? context.TraceIdentifier;

                proxiedRequest.Headers.TryAddWithoutValidation("trace-id", traceId); // the gateway's, never the client's

                foreach (var name in ForwardedRequestHeaders)
                {
                    if (context.Request.Headers.TryGetValue(name, out var values))
                    {
                        proxiedRequest.Headers.TryAddWithoutValidation(name, values.ToArray());
                    }
                }

                // Forward a body whenever the client sent one, for any method (some DELETE endpoints take a body)
                if (context.Features.Get<IHttpRequestBodyDetectionFeature>()?.CanHaveBody == true)
                {
                    proxiedRequest.Content = new StreamContent(context.Request.Body);
                    proxiedRequest.Content.Headers.ContentLength = context.Request.ContentLength;
                    if (context.Request.ContentType is { } contentType)
                    {
                        // Unvalidated so parameters (charset, multipart boundary) pass through unchanged
                        proxiedRequest.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);
                    }
                }

                var response = await client.SendAsync(proxiedRequest, context.RequestAborted);

                context.Response.StatusCode = (int)response.StatusCode;
                foreach (var name in ReturnedResponseHeaders)
                {
                    if (response.Headers.TryGetValues(name, out var values))
                    {
                        context.Response.Headers[name] = values.ToArray();
                    }
                }
                foreach (var name in ReturnedContentHeaders)
                {
                    if (response.Content.Headers.TryGetValues(name, out var values))
                    {
                        context.Response.Headers[name] = values.ToArray();
                    }
                }

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    await responseStream.CopyToAsync(context.Response.Body, context.RequestAborted);
                }

                //_logger.LogInformation($"Request to {targetUrl} successfully forwarded and response received.");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"HTTP Request error during forwarding to {targetUrl}");
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"Error forwarding request to the microservice.\"}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during forwarding to {targetUrl}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"Unexpected error occurred while forwarding request.\"}");
            }
        }
    }
}
