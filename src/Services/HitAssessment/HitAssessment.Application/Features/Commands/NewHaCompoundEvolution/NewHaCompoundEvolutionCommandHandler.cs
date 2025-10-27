using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.HitAssessment;
using Daikon.Shared.APIClients.MLogix;
using Daikon.Shared.DTO.MLogix;
using HitAssessment.Application.Contracts.Persistence;
using HitAssessment.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionCommandHandler : IRequestHandler<NewHaCompoundEvolutionCommand, NewHaCompoundEvolutionResDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewHaCompoundEvolutionCommandHandler> _logger;
        private readonly IHaCompoundEvolutionRepository _haCompoundEvoRepository;

        private readonly IEventSourcingHandler<HaAggregate> _haEventSourcingHandler;
        private readonly IMLogixAPI _mLogixAPIService;
        public NewHaCompoundEvolutionCommandHandler(ILogger<NewHaCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<HaAggregate> haEventSourcingHandler,
            IHaCompoundEvolutionRepository haCompoundEvoRepository,
            IMLogixAPI mLogixAPIService,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _haCompoundEvoRepository = haCompoundEvoRepository ?? throw new ArgumentNullException(nameof(haCompoundEvoRepository));
            _haEventSourcingHandler = haEventSourcingHandler ?? throw new ArgumentNullException(nameof(haEventSourcingHandler));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));
        }

        public async Task<NewHaCompoundEvolutionResDTO> Handle(NewHaCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Handling NewHaCompoundEvolutionCommand");
                // check if request.ImportMode is null, if it is, set it to false
                var dateCreated = request.DateCreated;
                request.SetCreateProperties(request.RequestorUserId);

                // handle dates
                request.ImportMode ??= false;
                request.DateCreated = (bool)request.ImportMode ? dateCreated : request.DateCreated;

                var haCEAddedEvent = _mapper.Map<HaCompoundEvolutionAddedEvent>(request);

                var aggregate = await _haEventSourcingHandler.GetByAsyncId(request.Id);

                var response = new NewHaCompoundEvolutionResDTO
                {
                    Id = request.Id,
                };
                if (request.MoleculeId is null || request.MoleculeId == Guid.Empty)
                {
                    if (request.RequestedSMILES is not null && request.RequestedSMILES.Value.Length > 0)
                    {
                        _logger.LogInformation("Will try to register molecule ...");
                        await RegisterMoleculeAndAssignToEvent(request, haCEAddedEvent, response);
                    }
                }
                else
                {
                    _logger.LogInformation("MoleculeId provided in request. Skipping molecule registration...");
                    haCEAddedEvent.MoleculeId = (Guid)request.MoleculeId;

                }

                aggregate.AddCompoundEvolution(haCEAddedEvent);

                await _haEventSourcingHandler.SaveAsync(aggregate);

                return response;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(HaAggregate), request.Id);
            }




        }
        private async Task RegisterMoleculeAndAssignToEvent(NewHaCompoundEvolutionCommand request, HaCompoundEvolutionAddedEvent eventToAdd, NewHaCompoundEvolutionResDTO response)
        {
            try
            {
                var mLogiXResponse = await _mLogixAPIService.RegisterMolecule(new RegisterMoleculeDTO
                {
                    Name = request.MoleculeName,
                    SMILES = request.RequestedSMILES,
                    DisclosureStage = Daikon.Shared.Constants.Workflow.Stages.HA
                });

                eventToAdd.MoleculeId = mLogiXResponse.Id;

                response.MoleculeId = mLogiXResponse.Id;
                response.Molecule = mLogiXResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling MLogixAPIService for SMILES: {SMILES}", request.RequestedSMILES);
                throw;
            }
        }
    }

}