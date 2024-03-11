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
        private readonly IMolDbAPIService _molDbAPIService;

        private readonly IEventSourcingHandler<HitCollectionAggregate> _hitCollectionEventSourcingHandler;


        public NewHitCommandHandler(ILogger<NewHitCommandHandler> logger,
            IEventSourcingHandler<HitCollectionAggregate> hitCollectionEventSourcingHandler,
            IHitRepository hitRepository, IMLogixAPIService mLogixAPIService, IMolDbAPIService molDbAPIService,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hitRepository = hitRepository ?? throw new ArgumentNullException(nameof(hitRepository));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
            _hitCollectionEventSourcingHandler = hitCollectionEventSourcingHandler ?? throw new ArgumentNullException(nameof(hitCollectionEventSourcingHandler));
            _molDbAPIService = molDbAPIService ?? throw new ArgumentNullException(nameof(molDbAPIService));
        }

        public async Task<Unit> Handle(NewHitCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var newHitAddedEvent = _mapper.Map<HitAddedEvent>(request);
                newHitAddedEvent.Author = request.RequestorUserId.ToString();

                var aggregate = await _hitCollectionEventSourcingHandler.GetByAsyncId(request.Id);

                // Some molecules are proprietary and cannot be registered in MLogix
                if (request.RequestedSMILES is not null && request.RequestedSMILES.Value.Length > 0)
                {
                    _logger.LogInformation("Will try to register molecule ...");
                    await RegisterMoleculeAndAssignToEvent(request, newHitAddedEvent);
                }

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
                var response = await _mLogixAPIService.RegisterCompound(new RegisterMoleculeRequest
                {
                    Name = request.MoleculeName,
                    RequestedSMILES = request.RequestedSMILES
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