using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Infrastructure;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Project.Application.Features.Commands.DeleteProjectCompoundEvolution
{
    public class DeleteProjectCompoundEvolutionCommandHandler : IRequestHandler<DeleteProjectCompoundEvolutionCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteProjectCompoundEvolutionCommandHandler> _logger;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvoRepository;

        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;

        public DeleteProjectCompoundEvolutionCommandHandler(ILogger<DeleteProjectCompoundEvolutionCommandHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectCompoundEvolutionRepository projectCompoundEvoRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectCompoundEvoRepository = projectCompoundEvoRepository ?? throw new ArgumentNullException(nameof(projectCompoundEvoRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));

        }

        public async Task<Unit> Handle(DeleteProjectCompoundEvolutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var projectCEDeletedEvent = _mapper.Map<ProjectCompoundEvolutionDeletedEvent>(request);

                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);
                
                aggregate.DeleteCompoundEvolution(projectCEDeletedEvent);

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