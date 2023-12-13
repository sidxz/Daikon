
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneResistanceMutationAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneResistanceMutationAddedEvent: {ResistanceMutationId}", @event.ResistanceMutationId);
            var resistanceMutation = new Domain.Entities.ResistanceMutation
            {
                Id = @event.ResistanceMutationId,
                GeneId = @event.GeneId,
                ResistanceMutationId = @event.ResistanceMutationId,
                Mutation = @event.Mutation,
                Isolate = @event.Isolate,
                ParentStrain = @event.ParentStrain,
                Compound = @event.Compound,
                ShiftInMIC = @event.ShiftInMIC,
                Organization = @event.Organization,
                Researcher = @event.Researcher,
                Reference = @event.Reference,
                Notes = @event.Notes,
                
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

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

            var resistanceMutation = await _geneResistanceMutationRepository.Read(@event.ResistanceMutationId);

            resistanceMutation.Mutation = @event.Mutation;
            resistanceMutation.Isolate = @event.Isolate;
            resistanceMutation.ParentStrain = @event.ParentStrain;
            resistanceMutation.Compound = @event.Compound;
            resistanceMutation.ShiftInMIC = @event.ShiftInMIC;
            resistanceMutation.Organization = @event.Organization;
            resistanceMutation.Researcher = @event.Researcher;
            resistanceMutation.Reference = @event.Reference;
            resistanceMutation.Notes = @event.Notes;
            
            resistanceMutation.IsModified = true;

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