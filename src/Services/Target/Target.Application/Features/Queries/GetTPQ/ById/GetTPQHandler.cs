
using CQRS.Core.Exceptions;
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Application.Features.Queries.GetTPQ.VMs;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.GetTPQ.ById
{
    public class GetTPQHandler : IRequestHandler<GetTPQQuery, PQResponseVM>
    {

        private readonly IPQResponseRepository _pqResponseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTPQHandler> _logger;

        public GetTPQHandler(IPQResponseRepository pqResponseRepository, IMapper mapper, ILogger<GetTPQHandler> logger)
        {
            _pqResponseRepository = pqResponseRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PQResponseVM> Handle(GetTPQQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pqResponse = await _pqResponseRepository.ReadById(request.Id)
                    ?? throw new ResourceNotFoundException("PQResponse", request.Id.ToString());
                var pqResponseVM = _mapper.Map<PQResponseVM>(pqResponse);

                var trackableEntities = new List<VMMeta> { pqResponseVM };
                (pqResponseVM.PageLastUpdatedDate, pqResponseVM.PageLastUpdatedUser) = VMUpdateTracker.CalculatePageLastUpdated(trackableEntities);
                return pqResponseVM;

            }
            catch (ResourceNotFoundException)
            {
                throw; // 404, not an error
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetTPQQuery");
                throw;
            }

        }
    }
}