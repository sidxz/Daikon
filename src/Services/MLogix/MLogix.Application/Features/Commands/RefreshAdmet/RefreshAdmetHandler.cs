using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Commands.RefreshAdmet
{
    public class RefreshAdmetHandler : IRequestHandler<RefreshAdmetCommand, AdmetBackfillTriggerVM>
    {
        private readonly ILogger<RefreshAdmetHandler> _logger;
        private readonly IMoleculeAPI _moleculeApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefreshAdmetHandler(ILogger<RefreshAdmetHandler> logger, IMoleculeAPI moleculeApi, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _moleculeApi = moleculeApi;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AdmetBackfillTriggerVM> Handle(RefreshAdmetCommand request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            _logger.LogInformation(
                "Triggering ADMET backfill (chunk_size={ChunkSize}, limit={Limit}, include_errors={IncludeErrors})",
                request.ChunkSize, request.Limit, request.IncludeErrors);

            return await _moleculeApi.TriggerAdmetBackfill(
                request.ChunkSize, request.Limit, request.IncludeErrors, headers);
        }
    }
}
