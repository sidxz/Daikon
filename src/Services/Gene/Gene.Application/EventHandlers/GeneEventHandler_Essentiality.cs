
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneEssentialityAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneEssentialityAddedEvent: {EssentialityId}", @event.EssentialityId);
            var essentiality = _mapper.Map<Essentiality>(@event);
            
             // Set Ids (swap)
            essentiality.Id = @event.EssentialityId;
            essentiality.GeneId = @event.Id;

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

            var existingEssentiality = await _geneEssentialityRepository.Read(@event.EssentialityId);
           
            var essentiality = _mapper.Map<Essentiality>(existingEssentiality);
            _mapper.Map(@event, essentiality);

            essentiality.Id = @event.EssentialityId;
            essentiality.GeneId = @event.Id;

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