
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneUnpublishedStructuralInformationAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneUnpublishedStructuralInformationAddedEvent: {UnpublishedStructuralInformationId}", @event.UnpublishedStructuralInformationId);
            var unpublishedStructuralInformation = new Domain.Entities.UnpublishedStructuralInformation
            {
                Id = @event.UnpublishedStructuralInformationId,
                GeneId = @event.GeneId,
                UnpublishedStructuralInformationId = @event.UnpublishedStructuralInformationId,
                Organization = @event.Organization,
                Method = @event.Method,
                Resolution = @event.Resolution,
                Ligands = @event.Ligands,
                Researcher = @event.Researcher,
                Reference = @event.Reference,
                Notes = @event.Notes,
                URL = @event.URL,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try
            {
                await _geneUnpublishedStructuralInformationRepository.AddUnpublishedStructuralInformation(unpublishedStructuralInformation);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneUnpublishedStructuralInformationCreatedEvent Error creating unpublishedStructuralInformation", ex);
            }
        }

        public async Task OnEvent(GeneUnpublishedStructuralInformationUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneUnpublishedStructuralInformationUpdatedEvent: {UnpublishedStructuralInformationId}", @event.UnpublishedStructuralInformationId);

            var unpublishedStructuralInformation = await _geneUnpublishedStructuralInformationRepository.Read(@event.UnpublishedStructuralInformationId);

            unpublishedStructuralInformation.Organization = @event.Organization;
            unpublishedStructuralInformation.Method = @event.Method;
            unpublishedStructuralInformation.Resolution = @event.Resolution;
            unpublishedStructuralInformation.Ligands = @event.Ligands;
            unpublishedStructuralInformation.Researcher = @event.Researcher;
            unpublishedStructuralInformation.Reference = @event.Reference;
            unpublishedStructuralInformation.Notes = @event.Notes;
            unpublishedStructuralInformation.URL = @event.URL;
            unpublishedStructuralInformation.IsModified = true;

            try
            {
                await _geneUnpublishedStructuralInformationRepository.UpdateUnpublishedStructuralInformation(unpublishedStructuralInformation);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneUnpublishedStructuralInformationUpdatedEvent Error updating unpublishedStructuralInformation with id @event.UnpublishedStructuralInformationId", ex);
            }
        }

        public async Task OnEvent(GeneUnpublishedStructuralInformationDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneUnpublishedStructuralInformationDeletedEvent: {UnpublishedStructuralInformationId}", @event.UnpublishedStructuralInformationId);
            try
            {
                await _geneUnpublishedStructuralInformationRepository.DeleteUnpublishedStructuralInformation(@event.UnpublishedStructuralInformationId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneUnpublishedStructuralInformationDeletedEvent Error deleting unpublishedStructuralInformation with id @event.UnpublishedStructuralInformationId", ex);
            }
        }
    }
}