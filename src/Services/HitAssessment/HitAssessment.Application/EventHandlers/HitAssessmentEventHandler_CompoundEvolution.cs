
using CQRS.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Daikon.Events.HitAssessment;
using HitAssessment.Domain.Entities;

namespace HitAssessment.Application.EventHandlers
{
    public partial class HitAssessmentEventHandler : IHitAssessmentEventHandler
    {

        public async Task OnEvent(HaCompoundEvolutionAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: HaCompoundEvolutionAddedEvent: HitAssessmentId {Id}, HaCEId {CompoundEvolutionId}", @event.Id, @event.CompoundEvolutionId);

            var compoundEvolution = _mapper.Map<HaCompoundEvolution>(@event);

            // Override compoundEvolution.Id to be the same as compoundEvolution.CompoundEvolutionId 
            // as @event.Id refers to HitAssessment (Aggregate Id) which is auto-mapped by the mapper
            // In MongoDb, we want to use compoundEvolutionId as the Id of the entity
            compoundEvolution.Id = @event.CompoundEvolutionId;

            // Set the HitAssessmentId to be the same as the Aggregate Id to maintain a 
            // relationship between HitAssessment and hACE
            compoundEvolution.HitAssessmentId = @event.Id;

            try
            {
                await _haCompoundEvolutionRepository.CreateHaCompoundEvolution(compoundEvolution);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "Error occurred while creating ha compound evolution run for HaCompoundEvolutionAddedEvent", ex);
            }
        }

        public async Task OnEvent(HaCompoundEvolutionUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: HaCompoundEvolutionUpdatedEvent: HitAssessmentId {Id}, HaCEId {CompoundEvolutionId}", @event.Id, @event.CompoundEvolutionId);
            var existingHaCompoundEvolution = await _haCompoundEvolutionRepository.ReadHaCompoundEvolutionById(@event.CompoundEvolutionId);

            if (existingHaCompoundEvolution == null)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating ha compound evolution {@event.CompoundEvolutionId} for HaCompoundEvolutionUpdatedEvent", new Exception("Ha compound evolution not found"));
            }

            var compoundEvolution = _mapper.Map<HaCompoundEvolution>(existingHaCompoundEvolution);
            _mapper.Map(@event, compoundEvolution);

            compoundEvolution.Id = @event.CompoundEvolutionId;
            compoundEvolution.HitAssessmentId = @event.Id;

            // Preserve the original creation date and creator
            compoundEvolution.CreatedById = existingHaCompoundEvolution.CreatedById;
            compoundEvolution.DateCreated = existingHaCompoundEvolution.DateCreated;
            
            try
            {
                await _haCompoundEvolutionRepository.UpdateHaCompoundEvolution(compoundEvolution);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while updating ha compound evolution {@event.CompoundEvolutionId} for HaCompoundEvolutionUpdatedEvent", ex);
            }
        }

        public async Task OnEvent(HaCompoundEvolutionDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: HaCompoundEvolutionDeletedEvent: {Id}", @event.CompoundEvolutionId);

            try
            {
                await _haCompoundEvolutionRepository.DeleteHaCompoundEvolution(@event.CompoundEvolutionId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"Error occurred while deleting ha compound evolution {@event.CompoundEvolutionId} for HaCompoundEvolutionDeletedEvent", ex);
            }
        }
    }
}