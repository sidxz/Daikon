
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneUnpublishedStructuralInformationAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneUnpublishedStructuralInformationAddedEvent: {UnpublishedStructuralInformationId}", @event.UnpublishedStructuralInformationId);
            var unpublishedStructuralInformation = _mapper.Map<UnpublishedStructuralInformation>(@event);

            unpublishedStructuralInformation.Id = @event.UnpublishedStructuralInformationId;
            unpublishedStructuralInformation.GeneId = @event.Id;

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

            var existingUsi = await _geneUnpublishedStructuralInformationRepository.Read(@event.UnpublishedStructuralInformationId);
            var unpublishedStructuralInformation = _mapper.Map<UnpublishedStructuralInformation>(existingUsi);
            _mapper.Map(@event, unpublishedStructuralInformation);

            unpublishedStructuralInformation.Id = @event.UnpublishedStructuralInformationId;
            unpublishedStructuralInformation.GeneId = @event.Id;

            // Preserve the original creation date and creator
            unpublishedStructuralInformation.CreatedById = existingUsi.CreatedById;
            unpublishedStructuralInformation.DateCreated = existingUsi.DateCreated;

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