
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;

namespace MLogix.Application.Features.Commands.GenerateParentBatch
{
    public class GenerateParentBatchHandler : IRequestHandler<GenerateParentBatchCommand, Unit>
    {

        private readonly ILogger<GenerateParentBatchHandler> _logger;
        private readonly IMoleculeAPI _moleculeApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenerateParentBatchHandler(ILogger<GenerateParentBatchHandler> logger, IMoleculeAPI moleculeApi, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _moleculeApi = moleculeApi;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(GenerateParentBatchCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            try
            {
                await _moleculeApi.BatchCreateParents(headers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateParentBatchHandler");
            }
            
            return Unit.Value;
        }
    }
}