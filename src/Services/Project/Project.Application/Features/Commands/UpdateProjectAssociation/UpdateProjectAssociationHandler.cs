using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.Core.Exceptions;
using CQRS.Core.Handlers;
using Daikon.Events.Project;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.Application.Contracts.Persistence;
using Project.Domain.Aggregates;

namespace Project.Application.Features.Commands.UpdateProjectAssociation
{
    public class UpdateProjectAssociationHandler:  IRequestHandler<UpdateProjectAssociationCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProjectAssociationHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IEventSourcingHandler<ProjectAggregate> _projectEventSourcingHandler;

        public UpdateProjectAssociationHandler(ILogger<UpdateProjectAssociationHandler> logger,
            IEventSourcingHandler<ProjectAggregate> projectEventSourcingHandler,
            IProjectRepository projectRepository,
            IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectEventSourcingHandler = projectEventSourcingHandler ?? throw new ArgumentNullException(nameof(projectEventSourcingHandler));
        }

        public async Task<Unit> Handle(UpdateProjectAssociationCommand request, CancellationToken cancellationToken)
        {
            // fetch existing project
            var existingProject = await _projectRepository.ReadProjectById(request.Id);
            if (existingProject == null)
            {
                _logger.LogWarning("Project not found: {Id}", request.Id);
                throw new ResourceNotFoundException(nameof(Project), request.Id);
            }

            if (existingProject.HaId == request.HaId)
            {
                _logger.LogWarning("Project not changed: {Id}", request.Id);
                throw new InvalidOperationException(nameof(Project));
            }

            var now = DateTime.UtcNow;

            request.DateModified = now;
            request.IsModified = true;


            var projectAssociationUpdatedEvent = _mapper.Map<ProjectAssociationUpdatedEvent>(request);

            try
            {
                var aggregate = await _projectEventSourcingHandler.GetByAsyncId(request.Id);

                aggregate.UpdateHaAssociation(projectAssociationUpdatedEvent);

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