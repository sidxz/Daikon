
using CQRS.Core.Exceptions;
using Daikon.Events.Project;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Projects;
using Microsoft.Extensions.Logging;

namespace Horizon.Application.Handlers
{
    public class ProjectEventHandler : IProjectEventHandler
    {
        private readonly ILogger<ProjectEventHandler> _logger;
        private readonly IProjectRepo _graphRepository;

        public ProjectEventHandler(ILogger<ProjectEventHandler> logger, IProjectRepo graphRepository)
        {
            _logger = logger;
            _graphRepository = graphRepository;
        }
        public Task OnEvent(ProjectCreatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ProjectCreatedEvent: {@event.Id} {@event.Name}");

            var project = new Project
            {
                UniId = @event.Id.ToString(),
                HitAssessmentId = @event.Id.ToString(),
                Name = @event.Name,
                Status = @event.Status,
                Stage = @event.Stage,
                IsProjectComplete = @event.IsProjectComplete,
                IsProjectRemoved = @event.IsProjectRemoved,
                PrimaryMoleculeId = @event.CompoundId.ToString(),
                HitMoleculeId = @event.HitCompoundId.ToString(),
                OrgId = @event.PrimaryOrgId.ToString(),
                DateCreated = @event.DateCreated,
                DateModified = @event.DateModified,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft
            };

            try
            {
                return _graphRepository.Create(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectCreatedEvent Error creating hit assessment", ex);
            }
        }

        public Task OnEvent(ProjectUpdatedEvent @event)
        {
            var project = new Project
            {
                UniId = @event.Id.ToString(),
                HitAssessmentId = @event.Id.ToString(),
                Name = @event.Name,
                Status = @event.Status,
                Stage = @event.Stage,
                IsProjectComplete = @event.IsProjectComplete,
                IsProjectRemoved = @event.IsProjectRemoved,
                PrimaryMoleculeId = @event.CompoundId.ToString(),
                HitMoleculeId = @event.HitCompoundId.ToString(),
                OrgId = @event.PrimaryOrgId.ToString(),
                DateCreated = @event.DateCreated,
                DateModified = @event.DateModified,
                IsModified = @event.IsModified,
                IsDraft = @event.IsDraft
            };

            try
            {
                return _graphRepository.Update(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectUpdatedEvent Error updating hit assessment", ex);
            }
        }

        public Task OnEvent(ProjectDeletedEvent @event)
        {
            try
            {
                return _graphRepository.Delete(@event.Id.ToString());
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectDeletedEvent Error deleting hit assessment", ex);
            }
        }
    }
}