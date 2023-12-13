
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneCrispriStrainAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneCrispriStrainAddedEvent: {CrispriStrainId}", @event.CrispriStrainId);
            var crispriStrain = new Domain.Entities.CrispriStrain
            {
                Id = @event.CrispriStrainId,
                GeneId = @event.GeneId,
                CrispriStrainId = @event.CrispriStrainId,
                CrispriStrainName = @event.CrispriStrainName,
                Notes = @event.Notes,
                DateCreated = DateTime.UtcNow,
                IsModified = false,
                IsDraft = false
            };

            try
            {
                await _geneCrispriStrainRepository.AddCrispriStrain(crispriStrain);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCrispriStrainCreatedEvent Error creating crispriStrain", ex);
            }
        }

        public async Task OnEvent(GeneCrispriStrainUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneCrispriStrainUpdatedEvent: {CrispriStrainId}", @event.CrispriStrainId);

            var crispriStrain = _geneCrispriStrainRepository.Read(@event.CrispriStrainId).Result;

            crispriStrain.CrispriStrainName = @event.CrispriStrainName;
            crispriStrain.Notes = @event.Notes;
            crispriStrain.IsModified = true;

            try
            {
                await _geneCrispriStrainRepository.UpdateCrispriStrain(crispriStrain);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCrispriStrainUpdatedEvent Error updating crispriStrain with id @event.CrispriStrainId", ex);
            }
        }

        public async Task OnEvent(GeneCrispriStrainDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneCrispriStrainDeletedEvent: {CrispriStrainId}", @event.CrispriStrainId);
            try
            {
                await _geneCrispriStrainRepository.DeleteCrispriStrain(@event.CrispriStrainId);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "GeneCrispriStrainDeletedEvent Error deleting crispriStrain with id @event.CrispriStrainId", ex);
            }
        }
    }
}