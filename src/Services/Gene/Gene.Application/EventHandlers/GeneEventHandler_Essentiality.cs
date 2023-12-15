
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneEssentialityAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityAddedEvent: {EssentialityId}", @event.EssentialityId);
            var essentiality = new Domain.Entities.Essentiality
            {
                Id = @event.EssentialityId,
                GeneId = @event.GeneId,
                EssentialityId = @event.EssentialityId,
                Classification = @event.Classification,
                Condition = @event.Condition,
                Method = @event.Method,
                Reference = @event.Reference,
                Note = @event.Note,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try
            {
                await _geneEssentialityRepository.AddEssentiality(essentiality);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneEssentialityCreatedEvent Error creating essentiality", ex);
            }
        }

        public async Task OnEvent(GeneEssentialityUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityUpdatedEvent: {EssentialityId}", @event.EssentialityId);

            var essentiality = await _geneEssentialityRepository.Read(@event.EssentialityId);

            essentiality.Classification = @event.Classification;
            essentiality.Condition = @event.Condition;
            essentiality.Method = @event.Method;
            essentiality.Reference = @event.Reference;
            essentiality.Note = @event.Note;
            essentiality.IsModified = true;

            try
            {
                await _geneEssentialityRepository.UpdateEssentiality(essentiality);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneEssentialityUpdatedEvent Error updating essentiality with id @event.EssentialityId", ex);
            }
        }

        public async Task OnEvent(GeneEssentialityDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityDeletedEvent: {EssentialityId}", @event.EssentialityId);
            try
            {
                await _geneEssentialityRepository.DeleteEssentiality(@event.EssentialityId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneEssentialityDeletedEvent Error deleting essentiality with id @event.EssentialityId", ex);
            }
        }
    }
}