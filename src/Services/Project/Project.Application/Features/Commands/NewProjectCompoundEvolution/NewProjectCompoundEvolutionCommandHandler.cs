using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Infrastructure;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Application.Features.Commands.NewHaCompoundEvolution;
using Project.Application.DTOs.MLogixAPI;
using Project.Application.Features.Queries.GetHitAssessment;


namespace Project.Application.Features.Commands.NewProjectCompoundEvolution
{
    public class NewProjectCompoundEvolutionCommandHandler : IRequestHandler<NewProjectCompoundEvolutionCommand, NewProjectCompoundEvolutionResDTO>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewProjectCompoundEvolutionCommandHandler> _logger;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvoRepository;

        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;
        private readonly IMLogixAPIService _mLogixAPIService;

        public NewProjectCompoundEvolutionCommandHandler(ILogger<NewProjectCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectCompoundEvolutionRepository projectCompoundEvoRepository,
            IMapper mapper, IMLogixAPIService mLogixAPIService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectCompoundEvoRepository = projectCompoundEvoRepository ?? throw new ArgumentNullException(nameof(projectCompoundEvoRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
            _mLogixAPIService = mLogixAPIService ?? throw new ArgumentNullException(nameof(mLogixAPIService));

        }

        public async Task<NewProjectCompoundEvolutionResDTO> Handle(NewProjectCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var compoundEvoAddedEvent = _mapper.Map<ProjectCompoundEvolutionAddedEvent>(request);

                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);

                var response = new NewProjectCompoundEvolutionResDTO
                {
                    Id = request.Id,
                };



                if (request.MoleculeId is null || request.MoleculeId == Guid.Empty)
                {
                    if (request.RequestedSMILES is not null && request.RequestedSMILES.Value.Length > 0)
                    {
                        _logger.LogInformation("Will try to register molecule ...");
                        await RegisterMoleculeAndAssignToEvent(request, compoundEvoAddedEvent, response);
                    }
                }
                else
                {
                    _logger.LogInformation("MoleculeId provided in request. Skipping molecule registration...");
                    compoundEvoAddedEvent.MoleculeId = (Guid)request.MoleculeId;

                }

                aggregate.AddCompoundEvolution(compoundEvoAddedEvent);

                await _projectEventSourcingHandler.SaveAsync(aggregate);

                return response;
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ProjectAggregate), request.Id);
            }
        }


        private async Task RegisterMoleculeAndAssignToEvent(NewProjectCompoundEvolutionCommand request, ProjectCompoundEvolutionAddedEvent eventToAdd, NewProjectCompoundEvolutionResDTO response)
        {
            try
            {
                var mLogiXResponse = await _mLogixAPIService.RegisterCompound(new RegisterMoleculeRequest
                {
                    Name = request.MoleculeName,
                    RequestedSMILES = request.RequestedSMILES
                });

                eventToAdd.MoleculeId = mLogiXResponse.Id;
                response.MoleculeId = mLogiXResponse.Id;
                response.Molecule = _mapper.Map<MoleculeVM>(mLogiXResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling MLogixAPIService for SMILES: {SMILES}", request.RequestedSMILES);
                throw;
            }
        }
    }

}
