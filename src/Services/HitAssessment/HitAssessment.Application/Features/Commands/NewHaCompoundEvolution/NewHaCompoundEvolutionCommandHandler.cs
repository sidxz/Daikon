using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.HitAssessment;
using HitAssessment.Application.Contracts.Infrastructure;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Application.DTOs.MLogixAPI;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionCommandHandler : IRequestHandler<NewHaCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewHaCompoundEvolutionCommandHandler> _logger;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;

        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;
        private readonly IMLogixAPIService _mLogixAPIService;
        public NewHaCompoundEvolutionCommandHandler(ILogger<NewHaCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHaCompoundEvolutionRepository haCompoundEvoRepository,
            IMLogixAPIService mLogixAPIService,
            IMapper mapper, IMolDbAPIService molDbAPIService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
        }

        public async Task<Unit> Handle(NewHaCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var haCEAddedEvent = _mapper.Map<HaCompoundEvolutionAddedEvent>(request);

                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);
               

                if (request.MoleculeId is null || request.MoleculeId == Guid.Empty)
                {
                    if (request.RequestedSMILES is not null && request.RequestedSMILES.Value.Length > 0)
                    {
                        _logger.LogInformation("Will try to register molecule ...");
                        await RegisterMoleculeAndAssignToEvent(request, haCEAddedEvent);
                    }
                }
                else
                {
                    _logger.LogInformation("MoleculeId provided in request. Skipping molecule registration...");
                     haCEAddedEvent.MoleculeId = (Guid)request.MoleculeId;
                    
                }

                aggregate.AddCompoundEvolution(haCEAddedEvent);

                await _haEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HaAggregate), request.Id);
            }

            
            return Unit.Value;
        }
        private async Task RegisterMoleculeAndAssignToEvent(NewHaCompoundEvolutionCommand request, HaCompoundEvolutionAddedEvent eventToAdd)
        {
            try
            {
                var response = await _mLogixAPIService.RegisterCompound(new RegisterMoleculeRequest
                {
                    Name = request.MoleculeName,
                    RequestedSMILES = request.RequestedSMILES
                });

                eventToAdd.MoleculeId = response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling MLogixAPIService for SMILES: {SMILES}", request.RequestedSMILES);
                throw;
            }
        }
    }

}