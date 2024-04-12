
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneProteinProductionAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinProductionAddedEvent: {ProteinProductionId}", @event.ProteinProductionId);
            var proteinProduction = _mapper.Map<ProteinProduction>(@event);

            // Set Ids
            proteinProduction.Id = @event.ProteinProductionId;
            proteinProduction.GeneId = @event.Id;

            try
            {
                await _geneProteinProductionRepository.AddProteinProduction(proteinProduction);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneProteinProductionCreatedEvent Error creating protein production", ex);
            }

        }

        public async Task OnEvent(GeneProteinProductionUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinProductionUpdatedEvent: {ProteinProductionId}", @event.ProteinProductionId);

            var existingProteinProduction = await _geneProteinProductionRepository.Read(@event.ProteinProductionId);
            var proteinProduction = _mapper.Map<ProteinProduction>(existingProteinProduction);
            _mapper.Map(@event, proteinProduction);

            proteinProduction.Id = @event.ProteinProductionId;
            proteinProduction.GeneId = @event.Id;

            try
            {
                await _geneProteinProductionRepository.UpdateProteinProduction(proteinProduction);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneProteinProductionUpdatedEvent Error updating protein production with id @event.ProteinProductionId", ex);
            }
        }

        public async Task OnEvent(GeneProteinProductionDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinProductionDeletedEvent: {ProteinProductionId}", @event.ProteinProductionId);
            try
            {
                await _geneProteinProductionRepository.DeleteProteinProduction(@event.ProteinProductionId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneProteinProductionDeletedEvent Error deleting protein production with id @event.ProteinProductionId", ex);
            }
        }
    }
}