
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
            var proteinProduction = new ProteinProduction
            {
                Id = @event.ProteinProductionId,
                GeneId = @event.GeneId,
                ProteinProductionId = @event.ProteinProductionId,
                Production = @event.Production,
                Method = @event.Method,
                Purity = @event.Purity,
                DateProduced = @event.DateProduced,
                PMID = @event.PMID,
                Notes = @event.Notes,
                URL = @event.URL,

                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

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

            var proteinProduction = await _geneProteinProductionRepository.Read(@event.ProteinProductionId);

            proteinProduction.Production = @event.Production;
            proteinProduction.Method = @event.Method;
            proteinProduction.Purity = @event.Purity;
            proteinProduction.DateProduced = @event.DateProduced;
            proteinProduction.PMID = @event.PMID;
            proteinProduction.Notes = @event.Notes;
            proteinProduction.URL = @event.URL;
            proteinProduction.IsModified = true;

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