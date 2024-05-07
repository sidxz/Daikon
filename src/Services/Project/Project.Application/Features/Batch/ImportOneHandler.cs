
using AutoMapper;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Daikon.Shared.Constants.AppProject;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;

namespace Project.Application.Features.Batch
{
    public class ImportOneHandler : IRequestHandler<ImportOneCommand, Unit>
    {

        private readonly IMapper _mapper;
        private readonly ILogger<ImportOneHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;
        private readonly IMediator _mediator;

        public ImportOneHandler(ILogger<ImportOneHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectRepository projectRepository,
            IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
        }
        public async Task<Unit> Handle(ImportOneCommand request, CancellationToken cancellationToken)
        {
            // check if name exists
            var existingProject = await _projectRepository.ReadProjectByName(request.Name);
            if (existingProject != null)
            {
                _logger.LogWarning("Project name already exists: {Name}", request.Name);
                throw new InvalidOperationException("Project name already exists");
            }

            var now = DateTime.UtcNow;

            // set HAPredictedStartDate to 10 days now if not set
            request.H2LPredictedStart ??= new DVariable<DateTime>(now);
            request.H2LStart ??= new DVariable<DateTime>(now);

            request.LOPredictedStart ??= new DVariable<DateTime>(now.AddDays(90));

            request.Stage ??= new DVariable<string>(nameof(ProjectStage.H2L));
            request.IsProjectComplete ??= false;
            request.IsProjectRemoved ??= false;

            var newProjectCreatedEvent = _mapper.Map<ProjectCreatedEvent>(request);

            var aggregate = new ProjectAggregate(newProjectCreatedEvent);

            await _projectEventSourcingHandler.SaveAsync(aggregate);

            foreach (var ceCommand in request.CompoundEvolutions)
            {
                var addedOnStage = ceCommand.Stage;
                ceCommand.Id = request.Id;
                ceCommand.CompoundEvolutionId = Guid.NewGuid();
                ceCommand.ImportMode = true;

                if (addedOnStage == "HA" ) 
                    continue;

                try
                {
                    await _mediator.Send(ceCommand, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating initial HaCompoundEvolution");
                    throw;
                }
            }
            return Unit.Value;
        }
    }
}