
using CQRS.Core.Exceptions;
using Daikon.Events.Project;
using Horizon.Application.Contracts.Persistence;
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
                ProjectId = @event.Id.ToString(),
                HitAssessmentId = @event.HaId.ToString(),
                Name = @event.Name,
                Status = @event.Status,
                Stage = @event.Stage,
                IsProjectComplete = @event.IsProjectComplete,
                IsProjectRemoved = @event.IsProjectRemoved,
                PrimaryMoleculeId = @event.CompoundId.ToString(),
                HitMoleculeId = @event.HitCompoundId.ToString(),
                OrgId = @event.PrimaryOrgId?.ToString() ?? "",

                DateCreated = @event?.DateCreated ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            try
            {
                return _graphRepository.Create(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectCreatedEvent Error creating project", ex);
            }
        }

        public Task OnEvent(ProjectUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ProjectUpdatedEvent: {@event.Id}");

            var project = new Project
            {
                UniId = @event.Id.ToString(),
                ProjectId = @event.Id.ToString(),
                Status = @event.Status,
                Stage = @event.Stage,
                IsProjectComplete = @event.IsProjectComplete,
                IsProjectRemoved = @event.IsProjectRemoved,
                OrgId = @event.PrimaryOrgId?.ToString() ?? "",

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            try
            {
                return _graphRepository.Update(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectUpdatedEvent Error updating project", ex);
            }
        }

        public Task OnEvent(ProjectRenamedEvent @event)
        {
            _logger.LogInformation($"Horizon: ProjectRenamedEvent: {@event.Id}");
            var project = new Project
            {
                UniId = @event.Id.ToString(),
                ProjectId = @event.Id.ToString(),
                Name = @event.Name,

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            try
            {
                return _graphRepository.Rename(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectRenamedEvent Error updating project", ex);
            }
        }


        public Task OnEvent(ProjectAssociationUpdatedEvent @event)
        {
            _logger.LogInformation($"Horizon: ProjectAssociationUpdatedEvent: {@event.Id}");

            var project = new Project
            {
                UniId = @event.Id.ToString(),
                ProjectId = @event.Id.ToString(),

                HitAssessmentId = @event.HaId.ToString(),
                PrimaryMoleculeId = @event.CompoundId.ToString(),
                HitMoleculeId = @event.HitCompoundId.ToString(),

                DateModified = @event?.DateModified ?? DateTime.Now,
                IsModified = @event?.IsModified ?? true,
                IsDraft = @event?.IsDraft ?? false
            };

            try
            {
                return _graphRepository.UpdateHitAssessmentAssociation(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectUpdatedEvent Error updating project", ex);
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
                throw new EventHandlerException(nameof(EventHandler), "ProjectDeletedEvent Error deleting project", ex);
            }
        }

        public Task OnEvent(ProjectCompoundEvolutionAddedEvent @event)
        {
            var projectCompoundEvolution = new ProjectCompoundEvolution
            {
                UniId = @event.Id.ToString(),
                ProjectId = @event.Id.ToString(),
                CompoundEvolutionId = @event.CompoundEvolutionId.ToString(),
                MoleculeId = @event.MoleculeId.ToString(),
                Stage = @event.Stage ?? "Unknown",
                DateCreated = @event.DateCreated ?? DateTime.Now,
                IsModified = @event.IsModified ?? false,
                IsDraft = @event.IsDraft ?? false
            };

            try
            {
                return _graphRepository.AddProjectCEvo(projectCompoundEvolution);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectCompoundEvolutionAddedEvent Error adding project compound evolution", ex);
            }
        }

        public Task OnEvent(ProjectCompoundEvolutionUpdatedEvent @event)
        {
            var projectCompoundEvolution = new ProjectCompoundEvolution
            {
                UniId = @event.Id.ToString(),
                ProjectId = @event.Id.ToString(),
                CompoundEvolutionId = @event.CompoundEvolutionId.ToString(),
                Stage = @event.Stage ?? "Unknown",
                DateModified = @event.DateModified ?? DateTime.Now,
                IsModified = @event.IsModified ?? true,
                IsDraft = @event.IsDraft ?? false
            };

            try
            {
                return _graphRepository.UpdateProjectCEvo(projectCompoundEvolution);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectCompoundEvolutionUpdatedEvent Error updating project compound evolution", ex);
            }
        }

        public Task OnEvent(ProjectCompoundEvolutionDeletedEvent @event)
        {
            try
            {
                return _graphRepository.DeleteProjectCEvo(@event.CompoundEvolutionId.ToString());
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectCompoundEvolutionDeletedEvent Error deleting project compound evolution", ex);
            }
        }
    }
}