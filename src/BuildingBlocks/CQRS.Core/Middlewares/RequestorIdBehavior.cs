using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using CQRS.Core.Command;
using CQRS.Core.Query;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using CQRS.Core.Extensions;

namespace CQRS.Core.Middlewares
{
    /// <summary>
    /// Middleware to assign RequestorUserId from HTTP headers to commands and queries.
    /// </summary>
    public class RequestorIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        // trace-id is stored with documents, so a caller-supplied value must look like an id
        private static readonly Regex SafeTraceId = new("^[A-Za-z0-9:-]{1,64}$", RegexOptions.Compiled);

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
        /// Gets the RequestorUserId from the user resolved off the validated bearer token (never from request headers).
        /// </summary>
        /// <returns>The user's Guid, or Guid.Empty when there is no signed-in user (e.g. background services).</returns>
        private Guid GetRequestorUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetAppUserId() ?? Guid.Empty;
        }

        private string GetTraceId()
        {
            if (_httpContextAccessor.HttpContext == null) return string.Empty;
            var traceId = _httpContextAccessor.HttpContext.Request.Headers["trace-id"].ToString();
            return SafeTraceId.IsMatch(traceId)
                ? traceId
                : Activity.Current?.TraceId.ToString() ?? _httpContextAccessor.HttpContext.TraceIdentifier;
        }
    }
}
