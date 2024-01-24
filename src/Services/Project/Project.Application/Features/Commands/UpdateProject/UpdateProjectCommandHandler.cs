using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Persistence;
using Project.Application.Features.Commands.UpdateProject;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Project.Application.Features.Commands.NewProject
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProjectCommandHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;

        public UpdateProjectCommandHandler(ILogger<UpdateProjectCommandHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectRepository projectRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var projectUpdatedEvent = _mapper.Map<ProjectUpdatedEvent>(request);

            try
            {
                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateProject(projectUpdatedEvent);

                await _projectEventSourcingHandler.SaveAsync(aggregate);
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.LogWarning(ex, "Aggregate not found");
                throw new ResourceNotFoundException(nameof(ProjectAggregate), request.Id); ;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling UpdateProjectCommandHandler");
                throw;
            }

            return Unit.Value;
        }
    }
}
