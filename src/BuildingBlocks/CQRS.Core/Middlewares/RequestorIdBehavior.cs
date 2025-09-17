using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using CQRS.Core.Command;
using CQRS.Core.Query;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CQRS.Core.Middlewares
{
    /// <summary>
    /// Middleware to assign RequestorUserId from HTTP headers to commands and queries.
    /// </summary>
    public class RequestorIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RequestorIdBehavior<TRequest, TResponse>> _logger;


        public RequestorIdBehavior(
            IHttpContextAccessor httpContextAccessor,
            ILogger<RequestorIdBehavior<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestorUserId = GetRequestorUserId();
                var traceId = GetTraceId();
                var path = _httpContextAccessor.HttpContext?.Request?.Path.Value ?? "<no-path>";
                var method = _httpContextAccessor.HttpContext?.Request?.Method ?? "<no-method>";


                // log requestorUserId id and traceId if needed
                _logger.LogInformation(
                        "🔛 TraceId {TraceId} | RequestorUserId: {RequestorUserId} | {Method} {Path}",
                        traceId, requestorUserId, method, path);

                switch (request)
                {
                    case BaseCommand baseCommand:
                        baseCommand.RequestorUserId = requestorUserId;
                        baseCommand.TraceId = traceId;
                        break;
                    case BaseQuery baseQuery:
                        baseQuery.RequestorUserId = requestorUserId;
                        baseQuery.TraceId = traceId;
                        break;
                }

                // Continue to the next delegate in the pipeline
                return await next();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism would be implemented here)
                throw new InvalidOperationException("An error occurred while processing the request.", ex);
            }
        }

        /// <summary>
        /// Extracts the RequestorUserId from the HTTP header, if available.
        /// </summary>
        /// <returns>The extracted Guid, or Guid.Empty if not found or invalid.</returns>
        private Guid GetRequestorUserId()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                // No HTTP context, typically in a background service.
                return Guid.Empty;
            }

            // Attempt to parse the AppUser-Id from headers
            var headers = _httpContextAccessor.HttpContext.Request.Headers;
            return Guid.TryParse(headers["AppUser-Id"], out var appUserId) ? appUserId : Guid.Empty;
        }

        private string GetTraceId()
        {
            if (_httpContextAccessor.HttpContext == null) return string.Empty;
            var headers = _httpContextAccessor.HttpContext.Request.Headers;
            return !string.IsNullOrWhiteSpace(headers["trace-id"])
                ? headers["trace-id"].ToString()
                : Activity.Current?.TraceId.ToString() ?? _httpContextAccessor.HttpContext.TraceIdentifier;
        }
    }
}
