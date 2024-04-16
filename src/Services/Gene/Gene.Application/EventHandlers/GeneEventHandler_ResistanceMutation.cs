
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneResistanceMutationAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneResistanceMutationAddedEvent: {ResistanceMutationId}", @event.ResistanceMutationId);
            var resistanceMutation = _mapper.Map<ResistanceMutation>(@event);

            resistanceMutation.Id = @event.ResistanceMutationId;
            resistanceMutation.GeneId = @event.Id;

            try
            {
                await _geneResistanceMutationRepository.AddResistanceMutation(resistanceMutation);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneResistanceMutationCreatedEvent Error creating resistanceMutation", ex);
            }
        }

        public async Task OnEvent(GeneResistanceMutationUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneResistanceMutationUpdatedEvent: {ResistanceMutationId}", @event.ResistanceMutationId);

            var existingResistanceMutation = await _geneResistanceMutationRepository.Read(@event.ResistanceMutationId);

            var resistanceMutation = _mapper.Map<ResistanceMutation>(existingResistanceMutation);
            _mapper.Map(@event, resistanceMutation);

            resistanceMutation.Id = @event.ResistanceMutationId;
            resistanceMutation.GeneId = @event.Id;
            // Preserve the original creation date and creator
            resistanceMutation.CreatedById = existingResistanceMutation.CreatedById;
            resistanceMutation.DateCreated = existingResistanceMutation.DateCreated;


            try
            {
                await _geneResistanceMutationRepository.UpdateResistanceMutation(resistanceMutation);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneResistanceMutationUpdatedEvent Error updating resistanceMutation with id @event.ResistanceMutationId", ex);
            }
        }

        public async Task OnEvent(GeneResistanceMutationDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneResistanceMutationDeletedEvent: {ResistanceMutationId}", @event.ResistanceMutationId);
            try
            {
                await _geneResistanceMutationRepository.DeleteResistanceMutation(@event.ResistanceMutationId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneResistanceMutationDeletedEvent Error deleting resistanceMutation with id @event.ResistanceMutationId", ex);
            }
        }
    }
}