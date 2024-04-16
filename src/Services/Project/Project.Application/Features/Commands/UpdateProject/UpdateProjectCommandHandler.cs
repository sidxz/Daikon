using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using Project.Application.Contracts.Persistence;
using Project.Application.Features.Commands.UpdateProject;
using Project.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
using Daikon.Shared.Constants.AppProject;
using CQRS.Core.Domain;


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

            // fetch existing project
            var existingProject = await _projectRepository.ReadProjectById(request.Id);
            if (existingProject == null)
            {
                _logger.LogWarning("Project not found: {Id}", request.Id);
                throw new ResourceNotFoundException(nameof(Project), request.Id);
            }

            var now = DateTime.UtcNow;

            // check if stage has changed
            if (existingProject.Stage != request.Stage)
            {
                if (request.Stage == nameof(ProjectStage.H2L))
                {
                    request.H2LStart = now;
                    request.LOPredictedStart = new DVariable<DateTime>(now.AddDays(90));
                }
                else if (request.Stage == nameof(ProjectStage.LO))
                {
                    request.LOStart = now;
                    request.SPPredictedStart = new DVariable<DateTime>(now.AddDays(90));
                }
                else if (request.Stage == nameof(ProjectStage.SP))
                {
                    request.SPStart = now;
                    request.INDPredictedStart = new DVariable<DateTime>(now.AddDays(90));
                }
                else if (request.Stage == nameof(ProjectStage.IND))
                {
                    request.INDStart = now;
                    request.P1PredictedStart = new DVariable<DateTime>(now.AddDays(90));
                }
                else if (request.Stage == nameof(ProjectStage.P1))
                {
                    request.P1Start = now;
                }
            }

            if ((existingProject.Priority ?? "") != (request.Priority ?? "")
                || (existingProject.Probability ?? "") != (request.Probability ?? "")
                || (existingProject.PriorityNote ?? "") != (request.PriorityNote ?? "")
                || (existingProject.ProbabilityNote ?? "") != (request.ProbabilityNote ?? ""))
            {
                request.PPLastStatusDate = now;
            }

            if ((existingProject.PmPriority ?? "") != (request.PmPriority ?? "")
                || (existingProject.PmProbability ?? "") != (request.PmProbability ?? "")
                || (existingProject.PmPriorityNote ?? "") != (request.PmPriorityNote ?? "")
                || (existingProject.PmProbabilityNote ?? "") != (request.PmProbabilityNote ?? ""))
            {
                request.PmPPLastStatusDate = now;
            }


            request.DateModified = now;
            request.IsModified = true;


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
