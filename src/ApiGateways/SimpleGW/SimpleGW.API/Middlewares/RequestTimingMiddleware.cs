using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleGW.API.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<RequestTimingMiddleware> _logger;
        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
            context.Response.Headers["trace-id"] = traceId;
            context.Response.Headers["trace-id"] = traceId;

            using (_logger.BeginScope(new Dictionary<string, object> { ["TraceId"] = traceId }))
            {
                var started = DateTime.UtcNow;
                _logger.LogInformation("⛸️ [START] TraceId {TraceId} | Path {Path} | Method {Method}",
                    traceId, context.Request.Path.Value, context.Request.Method);
                try
                {
                    await _next(context);
                    var durationMs = (DateTime.UtcNow - started).TotalMilliseconds;

                    _logger.LogInformation("🔚 [END] TraceId {TraceId} | Path {Path} | Duration {DurationMs} ms",
                        traceId, context.Request.Path.Value, durationMs);
                }
                catch (Exception ex)
                {
                    var durationMs = (DateTime.UtcNow - started).TotalMilliseconds;
                    _logger.LogError(ex,
                        "❌ [ERROR] TraceId {TraceId} | Path {Path} | Duration {DurationMs} ms | Exception: {Message}",
                        traceId, context.Request.Path.Value, durationMs, ex.Message);

                    throw;
                }


            }
        }
    }

}