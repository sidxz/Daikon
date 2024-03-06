using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using CQRS.Core.Command;
using CQRS.Core.Query;
namespace CQRS.Core.Middlewares
{
    public class RequestorIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestorIdBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is BaseCommand baseCommand)
            {
                baseCommand.RequestorUserId = Guid.TryParse(_httpContextAccessor.HttpContext.Request.Headers["AppUser-Id"], out var appUserId) ? appUserId : Guid.Empty;
            }
            else if (request is BaseQuery baseQuery)
            {
                baseQuery.RequestorUserId = Guid.TryParse(_httpContextAccessor.HttpContext.Request.Headers["AppUser-Id"], out var appUserId) ? appUserId : Guid.Empty;
            }

            // Call the next delegate/middleware in the pipeline
            return await next();
        }
    }

}