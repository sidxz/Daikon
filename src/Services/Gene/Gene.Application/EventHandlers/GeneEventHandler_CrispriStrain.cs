
using CQRS.Core.Exceptions;
using Daikon.Events.Gene;
using Gene.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Gene.Application.Query.EventHandlers
{
    public partial class GeneEventHandler
    {
        public async Task OnEvent(GeneCrispriStrainAddedEvent @event)
        {
            _logger.LogInformation("OnEvent: GeneCrispriStrainAddedEvent: {CrispriStrainId}", @event.CrispriStrainId);
            var crispriStrain = _mapper.Map<CrispriStrain>(@event);

            // Set Ids (swap)
            crispriStrain.Id = @event.CrispriStrainId;
            crispriStrain.GeneId = @event.Id;

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

            var existingCrispriStrain = await _geneCrispriStrainRepository.Read(@event.CrispriStrainId);

            var crispriStrain = _mapper.Map<CrispriStrain>(existingCrispriStrain);
            _mapper.Map(@event, crispriStrain);

            crispriStrain.Id = @event.CrispriStrainId;
            crispriStrain.GeneId = @event.Id;

             // Preserve the original creation date and creator
            crispriStrain.CreatedById = existingCrispriStrain.CreatedById;
            crispriStrain.DateCreated = existingCrispriStrain.DateCreated;

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