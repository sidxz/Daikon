
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Daikon.Events.Project;
using Project.Domain.Entities;

namespace Project.Application.EventHandlers
{
    public partial class ProjectEventHandler : IProjectEventHandler
    {

        public async Task OnEvent(ProjectCompoundEvolutionAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectCompoundEvolutionAddedEvent: ProjectId {Id}, ProjectCEId {CompoundEvolutionId}", @event.Id, @event.CompoundEvolutionId);

            var compoundEvolution = _mapper.Map<ProjectCompoundEvolution>(@event);

            // Override compoundEvolution.Id to be the same as compoundEvolution.CompoundEvolutionId 
            // as @event.Id refers to Project (Aggregate Id) which is auto-mapped by the mapper
            // In MongoDb, we want to use compoundEvolutionId as the Id of the entity
            compoundEvolution.Id = @event.CompoundEvolutionId;

            // Set the ProjectId to be the same as the Aggregate Id to maintain a 
            // relationship between Project and ProjectCE
            compoundEvolution.ProjectId = @event.Id;

            try
            {
                await _projectCompoundEvolutionRepository.CreateProjectCompoundEvolution(compoundEvolution);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "Error occurred while creating ha compound evolution run for ProjectCompoundEvolutionAddedEvent", ex);
            }
        }

        public async Task OnEvent(ProjectCompoundEvolutionUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectCompoundEvolutionUpdatedEvent: ProjectId {Id}, ProjectCEId {CompoundEvolutionId}", @event.Id, @event.CompoundEvolutionId);
            var existingProjectCompoundEvolution = await _projectCompoundEvolutionRepository.ReadProjectCompoundEvolutionById(@event.CompoundEvolutionId);

            if (existingProjectCompoundEvolution == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating ha compound evolution {@event.CompoundEvolutionId} for ProjectCompoundEvolutionUpdatedEvent", new Exception("Project compound evolution not found"));
            }

            var compoundEvolution = _mapper.Map<ProjectCompoundEvolution>(existingProjectCompoundEvolution);
            _mapper.Map(@event, compoundEvolution);
            
            compoundEvolution.Id = @event.CompoundEvolutionId;
            compoundEvolution.ProjectId = @event.Id;

             // Preserve the original creation date and creator
            compoundEvolution.CreatedById = compoundEvolution.CreatedById;
            compoundEvolution.DateCreated = compoundEvolution.DateCreated;

            try
            {
                await _projectCompoundEvolutionRepository.UpdateProjectCompoundEvolution(compoundEvolution);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating ha compound evolution {@event.CompoundEvolutionId} for ProjectCompoundEvolutionUpdatedEvent", ex);
            }
        }

        public async Task OnEvent(ProjectCompoundEvolutionDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: ProjectCompoundEvolutionDeletedEvent: {Id}", @event.CompoundEvolutionId);

            try
            {
                await _projectCompoundEvolutionRepository.DeleteProjectCompoundEvolution(@event.CompoundEvolutionId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting ha compound evolution {@event.CompoundEvolutionId} for ProjectCompoundEvolutionDeletedEvent", ex);
            }
        }
    }
}