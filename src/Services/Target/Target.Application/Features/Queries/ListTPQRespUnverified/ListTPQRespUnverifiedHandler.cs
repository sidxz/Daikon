
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.ListTPQRespUnverified
{
    public class ListTPQRespUnverifiedHandler : IRequestHandler<ListTPQRespUnverifiedQuery, List<PQResponse>>
    {

        private readonly IPQResponseRepository _pqResponseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ListTPQRespUnverifiedHandler> _logger;

        public ListTPQRespUnverifiedHandler(IPQResponseRepository pqResponseRepository, IMapper mapper, ILogger<ListTPQRespUnverifiedHandler> logger)
        {
            _pqResponseRepository = pqResponseRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<PQResponse>> Handle(ListTPQRespUnverifiedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pqResponses = await _pqResponseRepository.ReadPendingVerification();
                return pqResponses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUnverifiedPQResponses");
                throw;
            }
        }
    }
}