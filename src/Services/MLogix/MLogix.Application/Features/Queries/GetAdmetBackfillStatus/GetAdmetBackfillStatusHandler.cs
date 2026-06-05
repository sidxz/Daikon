using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Features.Queries.GetAdmetBackfillStatus
{
    public class GetAdmetBackfillStatusHandler : IRequestHandler<GetAdmetBackfillStatusQuery, AdmetBackfillCountsVM>
    {
        private readonly ILogger<GetAdmetBackfillStatusHandler> _logger;
        private readonly IMoleculeAPI _moleculeApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAdmetBackfillStatusHandler(ILogger<GetAdmetBackfillStatusHandler> logger, IMoleculeAPI moleculeApi, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _moleculeApi = moleculeApi;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AdmetBackfillCountsVM> Handle(GetAdmetBackfillStatusQuery request, CancellationToken cancellationToken)
        {
            var headers = _httpContextAccessor.HttpContext.Request.Headers
                        .ToDictionary(h => h.Key, h => h.Value.ToString());

            return await _moleculeApi.GetAdmetBackfillStatus(headers);
        }
    }
}
