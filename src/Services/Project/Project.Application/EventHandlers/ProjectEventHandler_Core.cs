
using AutoMapper;
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Project.Application.Contracts.Persistence;
using Daikon.Events.Project;

namespace Project.Application.EventHandlers
{
    public partial class ProjectEventHandler : IProjectEventHandler
    {

        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectEventHandler> _logger;
        private readonly IProjectCompoundEvolutionRepository _projectCompoundEvolutionRepository;

        public ProjectEventHandler(IProjectRepository projectRepository, IProjectCompoundEvolutionRepository projectCompoundEvolutionRepository,
                                    IMapper mapper, ILogger<ProjectEventHandler> logger)
        {
            _projectRepository = projectRepository;
            _projectCompoundEvolutionRepository = projectCompoundEvolutionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task OnEvent(ProjectCreatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectCreatedEvent: {Id}", @event.Id);
            var project = _mapper.Map<Domain.Entities.Project>(@event);
            project.Id = @event.Id;
            project.DateCreated = DateTime.UtcNow;
            project.IsModified = false;

            try
            {
                await _projectRepository.CreateProject(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "ProjectCreatedEvent Error creating project", ex);
            }
        }

        public async Task OnEvent(ProjectUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectUpdatedEvent: {Id}", @event.Id);
            var existingProject = await _projectRepository.ReadProjectById(@event.Id);

            if (existingProject == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ProjectUpdatedEvent Error updating project {@event.Id}", new Exception("Project not found"));
            }

            var project = _mapper.Map<Domain.Entities.Project>(existingProject);
            _mapper.Map(@event, project);

            project.DateCreated = existingProject.DateCreated;

            project.DateModified = DateTime.UtcNow;
            project.IsModified = true;

            try
            {
                await _projectRepository.UpdateProject(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ProjectUpdatedEvent Error updating project {@event.Id}", ex);
            }
        }

        public async Task OnEvent(ProjectAssociationUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectAssociationUpdatedEvent: {Id}", @event.Id);
            var existingProject = await _projectRepository.ReadProjectById(@event.Id);

            if (existingProject == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ProjectUpdatedEvent Error updating project {@event.Id}", new Exception("Project not found"));
            }

            var project = _mapper.Map<Domain.Entities.Project>(existingProject);
            _mapper.Map(@event, project);

            project.DateCreated = existingProject.DateCreated;

            project.DateModified = DateTime.UtcNow;
            project.IsModified = true;

            try
            {
                await _projectRepository.UpdateProject(project);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ProjectUpdatedEvent Error updating project {@event.Id}", ex);
            }
        }

        public async Task OnEvent(ProjectDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectDeletedEvent: {Id}", @event.Id);
            try
            {
                await _projectRepository.DeleteProject(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"ProjectDeletedEvent Error deleting project {@event.Id}", ex);
            }

        }

    }
}