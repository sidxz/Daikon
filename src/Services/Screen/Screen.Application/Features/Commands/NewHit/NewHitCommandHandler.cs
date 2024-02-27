using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Screens;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Infrastructure;
using Screen.Application.Contracts.Persistence;
using Screen.Application.DTOs.MLogixAPI;
using Screen.Domain.Aggregates;


namespace Screen.Application.Features.Commands.NewHit
{
    public class NewHitCommandHandler : IRequestHandler<NewHitCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewHitCommandHandler> _logger;
        private readonly IHitRepository _hitRepository;
        private readonly IMLogixAPIService _mLogixAPIService;

        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;


        public NewHitCommandHandler(ILogger<NewHitCommandHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitRepository hitRepository, IMLogixAPIService mLogixAPIService,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));

        }

        public async Task<Unit> Handle(NewHitCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var newHitAddedEvent = _mapper.Map<HitAddedEvent>(request);

                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);
                Guid compoundId;
                RegisterMoleculeResponseDTO moleculeRegistrationResponse;
                try {
                    var moleculeRegistrationRequest = new RegisterMoleculeRequest
                    {
                        Name = request.CompoundName,
                        RequestedSMILES = request.InitialCompoundStructure
                    };
                    moleculeRegistrationResponse = await _mLogixAPIService.RegisterCompound(moleculeRegistrationRequest);
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while calling MolDbAPI");
                    _logger.LogError(ex.Message);
                    throw new Exception(nameof(HitCollectionAggregate));
                }
                
                newHitAddedEvent.CompoundId = moleculeRegistrationResponse.Id;
                newHitAddedEvent.CompoundRegistrationId = moleculeRegistrationResponse.RegistrationId;
                
                aggregate.AddHit(newHitAddedEvent);

                await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.Id);
            }
            return Unit.Value;
        }
    }
}