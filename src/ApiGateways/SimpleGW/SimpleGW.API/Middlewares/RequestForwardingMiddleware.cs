
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SimpleGW.API.Middlewares
{
    public class RequestForwardingMiddleware
    {
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
                var client = _clientFactory.CreateClient();
                var proxiedRequest = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);
                // Ensure our trace-id is forwarded
                var traceId = context.Items["TraceId"] as string
                              ?? Activity.Current?.TraceId.ToString()
                              ?? context.TraceIdentifier;

                proxiedRequest.Headers.TryAddWithoutValidation("trace-id", traceId);

                // Copy headers, excluding specific ones
                foreach (var header in context.Request.Headers)
                {
                    if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) &&
                        !header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        proxiedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                    }
                }
                // set the Content-Type header to application/json for POST, PUT, and PATCH requests
                if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
                {
                    proxiedRequest.Content = new StreamContent(context.Request.Body);
                    proxiedRequest.Content.Headers.ContentType = new MediaTypeHeaderValue(context.Request.ContentType);
                }

                var response = await client.SendAsync(proxiedRequest, context.RequestAborted);

                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.ContentType = "application/json";
                foreach (var header in response.Headers)
                {
                    if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) &&
                        !header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) &&
                        !header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.Headers[header.Key] = header.Value.ToArray();
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
