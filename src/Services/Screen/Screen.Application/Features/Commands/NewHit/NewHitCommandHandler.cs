using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Screens;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.DTO.MLogix;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Domain.Aggregates;


namespace Screen.Application.Features.Commands.NewHit
{
    public class NewHitCommandHandler : IRequestHandler<NewHitCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<NewHitCommandHandler> _logger;
        private readonly IHitRepository _hitRepository;
        private readonly IMLogixAPI _mLogixAPIService;
        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;


        public NewHitCommandHandler(ILogger<NewHitCommandHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitRepository hitRepository, IMLogixAPI mLogixAPIService,
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
                request.SetCreateProperties(request.RequestorUserId);
                var newHitAddedEvent = _mapper.Map<HitAddedEvent>(request);
                newHitAddedEvent.Author = request.RequestorUserId.ToString();
                newHitAddedEvent.RequestedMoleculeName = request.MoleculeName;

                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);

                // Some molecules are proprietary and cannot be registered in MLogix


                _logger.LogInformation("Will try to register molecule ...");
                newHitAddedEvent.IsStructureDisclosed = !string.IsNullOrWhiteSpace(request.RequestedSMILES?.Value);
                await RegisterMoleculeAndAssignToEvent(request, newHitAddedEvent);


                aggregate.AddHit(newHitAddedEvent);

                await _hitCollectionEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HitCollectionAggregate), request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing request.");
                throw; // Consider a more specific exception if applicable
            }
            return Unit.Value;
        }

        private async Task RegisterMoleculeAndAssignToEvent(NewHitCommand request, HitAddedEvent eventToAdd)
        {
            try
            {
                var response = await _mLogixAPIService.RegisterMolecule(new RegisterMoleculeDTO
                {
                    Name = request.MoleculeName,
                    SMILES = request.RequestedSMILES
                });

                eventToAdd.MoleculeId = response.Id;
                eventToAdd.MoleculeRegistrationId = response.RegistrationId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling MLogixAPIService for SMILES: {SMILES}", request.RequestedSMILES);
                throw;
            }
        }
    }
}