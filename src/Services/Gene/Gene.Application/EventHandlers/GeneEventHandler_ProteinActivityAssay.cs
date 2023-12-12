
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneProteinActivityAssayAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneProteinActivityAssayAddedEvent: {ProteinActivityAssayId}", @event.ProteinActivityAssayId);
            var proteinActivityAssay = new Domain.Entities.ProteinActivityAssay
            {
                Id = @event.ProteinActivityAssayId,
                GeneId = @event.GeneId,
                ProteinActivityAssayId = @event.ProteinActivityAssayId,
                Assay = @event.Assay,
                Method = @event.Method,
                Throughput = @event.Throughput,
                PMID = @event.PMID,
                Reference = @event.Reference,
                URL = @event.URL,
                
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

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

            var proteinActivityAssay = _geneProteinActivityAssayRepository.Read(@event.ProteinActivityAssayId).Result;

            proteinActivityAssay.Assay = @event.Assay;
            proteinActivityAssay.Method = @event.Method;
            proteinActivityAssay.Throughput = @event.Throughput;
            proteinActivityAssay.PMID = @event.PMID;
            proteinActivityAssay.Reference = @event.Reference;
            proteinActivityAssay.URL = @event.URL;
            proteinActivityAssay.IsModified = true;

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