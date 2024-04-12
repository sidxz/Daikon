
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;
namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneExpansionPropAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneExpansionPropAddedEvent: {Id}", @event.Id);
            var geneExpansionProp = _mapper.Map<GeneExpansionProp>(@event);

            // Set Ids
            geneExpansionProp.Id = @event.ExpansionPropId;
            geneExpansionProp.GeneId = @event.Id;

            try
            {
                await _geneExpansionPropRepo.Create(geneExpansionProp);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(GeneEventHandler), "Error occurred while creating gene expansion prop for GeneExpansionPropAddedEvent", ex);
            }
        }

        public async Task OnEvent(GeneExpansionPropUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneExpansionPropUpdatedEvent: {Id}", @event.Id);
            var existingGeneExpansionProp = await _geneExpansionPropRepo.ReadById(@event.ExpansionPropId);

            var geneExpansionProp = _mapper.Map<GeneExpansionProp>(existingGeneExpansionProp);
            _mapper.Map(@event, geneExpansionProp);
            geneExpansionProp.Id = @event.ExpansionPropId;
            geneExpansionProp.GeneId = @event.Id;

            try
            {
                await _geneExpansionPropRepo.Update(geneExpansionProp);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(GeneEventHandler), "Error occurred while updating gene expansion prop for GeneExpansionPropUpdatedEvent", ex);
            }

        }

        public Task OnEvent(GeneExpansionPropDeletedEvent @event)
        {

            _logger.LogInformation("OnEvent: GeneExpansionPropDeletedEvent: {Id}", @event.Id);
            try
            {
                return _geneExpansionPropRepo.Delete(@event.ExpansionPropId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(GeneEventHandler), "Error occurred while deleting gene expansion prop for GeneExpansionPropDeletedEvent", ex);
            }

        }
    }
}