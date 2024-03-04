
using Daikon.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using UserStore.Domain.Entities;

namespace UserStore.Application.Features.Queries.GlobalValues.GetGlobalValues
{
    public class GetGlobalValuesHandler : IRequestHandler<GetGlobalValuesQuery, object>
    {
        private readonly ILogger<GetGlobalValuesHandler> _logger;

        public GetGlobalValuesHandler(ILogger<GetGlobalValuesHandler> logger)
        {
            _logger = logger;
        }

        public Task<object> Handle(GetGlobalValuesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var globalValue = ConstantsVM.Get();
                return Task.FromResult(globalValue);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get global values: {e.Message}");
            }
        }
    }
}