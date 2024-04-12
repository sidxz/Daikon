
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneProteinActivityAssayAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinActivityAssayAddedEvent: {ProteinActivityAssayId}", @event.ProteinActivityAssayId);
            var proteinActivityAssay = _mapper.Map<ProteinActivityAssay>(@event);

            // Set Ids
            proteinActivityAssay.Id = @event.ProteinActivityAssayId;
            proteinActivityAssay.GeneId = @event.Id;

            try
            {
                await _geneProteinActivityAssayRepository.AddProteinActivityAssay(proteinActivityAssay);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneProteinActivityAssayCreatedEvent Error creating proteinActivityAssay", ex);
            }
        }

        public async Task OnEvent(GeneProteinActivityAssayUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinActivityAssayUpdatedEvent: {ProteinActivityAssayId}", @event.ProteinActivityAssayId);

            var existingProteinActivityAssay = await _geneProteinActivityAssayRepository.Read(@event.ProteinActivityAssayId);

            var proteinActivityAssay = _mapper.Map<ProteinActivityAssay>(existingProteinActivityAssay);
            _mapper.Map(@event, proteinActivityAssay);

            proteinActivityAssay.Id = @event.ProteinActivityAssayId;
            proteinActivityAssay.GeneId = @event.Id;
            
            try
            {
                await _geneProteinActivityAssayRepository.UpdateProteinActivityAssay(proteinActivityAssay);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneProteinActivityAssayUpdatedEvent Error updating proteinActivityAssay with id @event.ProteinActivityAssayId", ex);
            }
        }

        public async Task OnEvent(GeneProteinActivityAssayDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinActivityAssayDeletedEvent: {ProteinActivityAssayId}", @event.ProteinActivityAssayId);
            try
            {
                await _geneProteinActivityAssayRepository.DeleteProteinActivityAssay(@event.ProteinActivityAssayId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneProteinActivityAssayDeletedEvent Error deleting proteinActivityAssay with id @event.ProteinActivityAssayId", ex);
            }
        }
    }
}