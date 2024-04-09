using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Infrastructure;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Domain.Entities;


namespace Project.Application.Features.Commands.UpdateProjectCompoundEvolution
{
    public class UpdateProjectCompoundEvolutionCommandHandler : IRequestHandler<UpdateProjectCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProjectCompoundEvolutionCommandHandler> _logger;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvoRepository;

        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;
        private readonly IMLogixAPIService _mLogixAPIService;

        public UpdateProjectCompoundEvolutionCommandHandler(ILogger<UpdateProjectCompoundEvolutionCommandHandler> logger,
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

        public async Task<Unit> Handle(UpdateProjectCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
           // fetch existing CE
            var existingCEvo = await _projectCompoundEvoRepository.ReadProjectCompoundEvolutionById(request.CompoundEvolutionId);

            if (existingCEvo == null)
            {
                throw new ResourceNotFoundException(nameof(ProjectCompoundEvolution), request.CompoundEvolutionId);
            }

             try
            {
                var compoundEvoUpdatedEvent = _mapper.Map<ProjectCompoundEvolutionUpdatedEvent>(request);

                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);

                // TODO (Future option) : check if molecule has been updated then register it
                // if (request.RequestedSMILES != existingCEvo.RequestedSMILES)
                // {
                //     if (request.RequestedSMILES is not null && request.RequestedSMILES.Value.Length > 0)
                //     {
                //         _logger.LogInformation("Will try to register molecule ...");
                //         await RegisterMoleculeAndAssignToEvent(request, compoundEvoUpdatedEvent);
                //     }
                //     else
                //     {
                //         throw new ArgumentNullException(nameof(request.RequestedSMILES));
                //     }
                // }
                // else
                // {
                //     compoundEvoUpdatedEvent.MoleculeId = existingCEvo.MoleculeId;
                // }

                aggregate.UpdateCompoundEvolution(compoundEvoUpdatedEvent);

                await _projectEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ProjectAggregate), request.Id);
            }
            return Unit.Value;
        }
    }

}