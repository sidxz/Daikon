using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using CQRS.Core.Command;
using CQRS.Core.Query;

namespace CQRS.Core.Middlewares
{
    /// <summary>
    /// Middleware to assign RequestorUserId from HTTP headers to commands and queries.
    /// </summary>
    public class RequestorIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestorIdBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            try
            {
                var requestorUserId = GetRequestorUserId();
                
                switch (request)
                {
                    case BaseCommand baseCommand:
                        baseCommand.RequestorUserId = requestorUserId;
                        break;
                    case BaseQuery baseQuery:
                        baseQuery.RequestorUserId = requestorUserId;
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
    }
}
