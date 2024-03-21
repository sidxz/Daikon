
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Target.Application.Contracts.Persistence;
using Target.Domain.Entities;

namespace Target.Application.Features.Queries.GetTPQ.ById
{
    public class GetTPQHandler : IRequestHandler<GetTPQQuery, PQResponse>
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

        public async Task<PQResponse> Handle(GetTPQQuery request, CancellationToken cancellationToken)
        {
           try 
           {
               var pqResponse = await _pqResponseRepository.ReadById(request.Id);
               return pqResponse;
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "An error occurred while handling GetTPQQuery");
               throw;
           }

        }
    }
}