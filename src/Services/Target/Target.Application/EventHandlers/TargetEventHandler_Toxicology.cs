
using CQRS.Core.Exceptions;
using Daikon.Events.Targets;
using Microsoft.Extensions.Logging;

namespace Target.Application.EventHandlers
{
    public partial class TargetEventHandler : ITargetEventHandler
    {

        public async Task OnEvent(TargetToxicologyAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetToxicologyAddedEvent: {Id}", @event.Id);
            var toxicology = _mapper.Map<Domain.Entities.Toxicology>(@event);
            // Set Ids (swap)
            toxicology.Id = @event.ToxicologyId;
            toxicology.TargetId = @event.Id;
            try
            {
                await _toxicologyRepo.Create(toxicology);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetToxicologyAddedEvent Error creating Toxicology", ex);
            }
        }

        public async Task OnEvent(TargetToxicologyUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetToxicologyUpdatedEvent: {Id}", @event.Id);
            var existingToxicology = await _toxicologyRepo.ReadById(@event.ToxicologyId);
            var toxicology = _mapper.Map<Domain.Entities.Toxicology>(existingToxicology);

            _mapper.Map(@event, toxicology);
            toxicology.Id = @event.ToxicologyId;
            toxicology.TargetId = @event.Id;

            // Preserve the original creation date and creator
            toxicology.CreatedById = existingToxicology.CreatedById;
            toxicology.DateCreated = existingToxicology.DateCreated;

            try
            {
                await _toxicologyRepo.Update(toxicology);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetToxicologyUpdatedEvent Error updating Toxicology", ex);
            }
        }

        public async Task OnEvent(TargetToxicologyDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetToxicologyDeletedEvent: {Id}", @event.Id);
            try
            {
                await _toxicologyRepo.Delete(@event.ToxicologyId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetToxicologyDeletedEvent Error deleting Toxicology", ex);
            }
        }
    }
}