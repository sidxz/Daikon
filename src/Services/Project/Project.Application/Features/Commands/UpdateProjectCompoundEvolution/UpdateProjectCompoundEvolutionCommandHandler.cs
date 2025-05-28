using AutoMapper;
using CQRS.Core.Exceptions;
using Daikon.EventStore.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Domain.Entities;
using System.Text.Json.Nodes;


namespace Project.Application.Features.Commands.UpdateProjectCompoundEvolution
{
    public class UpdateProjectCompoundEvolutionCommandHandler : IRequestHandler<UpdateProjectCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProjectCompoundEvolutionCommandHandler> _logger;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvoRepository;

        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;

        public UpdateProjectCompoundEvolutionCommandHandler(ILogger<UpdateProjectCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectCompoundEvolutionRepository projectCompoundEvoRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectCompoundEvoRepository = projectCompoundEvoRepository ?? throw new ArgumentNullException(nameof(projectCompoundEvoRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateProjectCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {

            request.SetUpdateProperties(request.RequestorUserId);

            // log stage
            // _logger.LogInformation("++++++++++++++++++++  UpdateProjectCompoundEvolutionCommand: {CompoundEvolutionId} {Stage}", request.CompoundEvolutionId, request.Stage);
            // fetch existing CE
            var existingCEvo = await _projectCompoundEvoRepository.ReadProjectCompoundEvolutionById(request.CompoundEvolutionId)
                        ?? throw new ResourceNotFoundException(nameof(ProjectCompoundEvolution), request.CompoundEvolutionId);
            try
            {

                var compoundEvoUpdatedEvent = _mapper.Map<ProjectCompoundEvolutionUpdatedEvent>(request);

                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateCompoundEvolution(compoundEvoUpdatedEvent);

                // _logger.LogInformation("------------JSON--------------");
                // _logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(compoundEvoUpdatedEvent));

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