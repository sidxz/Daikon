
using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.Shared.APIClients.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Entities;

namespace Screen.Application.Features.Queries.GetHit.ById
{
    public class GetHitByIdHandler : IRequestHandler<GetHitByIdCommand, Hit>
    {

        private readonly IMapper _mapper;
        private readonly IHitRepository _hitRepository;
        private readonly ILogger<GetHitByIdHandler> _logger;
        private readonly IMLogixAPI _mLogixAPIService;

        public GetHitByIdHandler(IMapper mapper, IHitRepository hitRepository, ILogger<GetHitByIdHandler> logger, 
        IMLogixAPI mLogixAPIService)
        {
            _mapper = mapper;
            _hitRepository = hitRepository;
            _logger = logger;
            _mLogixAPIService = mLogixAPIService;
        }

        public async Task<Hit> Handle(GetHitByIdCommand request, CancellationToken cancellationToken)
        {
            var hit = await _hitRepository.ReadHitById(request.Id);
            if (hit == null)
            {
                throw new AggregateNotFoundException(nameof(Hit));
            }
            return hit;
        }
    }
}