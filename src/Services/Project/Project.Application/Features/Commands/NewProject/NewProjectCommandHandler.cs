using AutoMapper;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Project.Application.Features.Commands.NewProject
{
    public class NewProjectCommandHandler : IRequestHandler<NewProjectCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NewProjectCommandHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;

        public NewProjectCommandHandler(ILogger<NewProjectCommandHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectRepository projectRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
        }

        public async Task<Unit> Handle(NewProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Handling NewProjectCommand: {request}");

                // check if name exists
                var existingProject = await _projectRepository.ReadProjectByName(request.Name);
                if (existingProject != null)
                {
                    _logger.LogWarning("Project name already exists: {Name}", request.Name);
                    throw new InvalidOperationException("Project name already exists");
                }

                var newProjectCreatedEvent = _mapper.Map<ProjectCreatedEvent>(request);

                var aggregate = new ProjectAggregate(newProjectCreatedEvent);

                await _projectEventSourcingHandler.SaveAsync(aggregate);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling NewProjectCommand");
                throw;
            }
        }
    }
}
