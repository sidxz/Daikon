
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneHypomorphAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneHypomorphAddedEvent: {HypomorphId}", @event.HypomorphId);
            var hypomorph = _mapper.Map<Hypomorph>(@event);

            // Set Ids
            hypomorph.Id = @event.HypomorphId;
            hypomorph.GeneId = @event.Id;
            
            try
            {
                await _geneHypomorphRepository.AddHypomorph(hypomorph);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneHypomorphCreatedEvent Error creating hypomorph", ex);
            }
        }

        public async Task OnEvent(GeneHypomorphUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneHypomorphUpdatedEvent: {HypomorphId}", @event.HypomorphId);

            var existingHypomorph = await _geneHypomorphRepository.Read(@event.HypomorphId);
            
            var hypomorph = _mapper.Map<Hypomorph>(existingHypomorph);

            _mapper.Map(@event, hypomorph);
            hypomorph.Id = @event.HypomorphId;
            hypomorph.GeneId = @event.Id;


            try
            {
                await _geneHypomorphRepository.UpdateHypomorph(hypomorph);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneHypomorphUpdatedEvent Error updating hypomorph with id @event.HypomorphId", ex);
            }
        }

        public async Task OnEvent(GeneHypomorphDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneHypomorphDeletedEvent: {HypomorphId}", @event.HypomorphId);
            try
            {
                await _geneHypomorphRepository.DeleteHypomorph(@event.HypomorphId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneHypomorphDeletedEvent Error deleting hypomorph with id @event.HypomorphId", ex);
            }
        }
    }
}