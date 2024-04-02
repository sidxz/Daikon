using CQRS.Core.Exceptions;
using Daikon.Events.Targets;
using Microsoft.Extensions.Logging;

namespace Target.Application.EventHandlers
{
    public partial class TargetEventHandler : ITargetEventHandler
    {

        public async Task OnEvent(TargetPromotionQuestionnaireSubmittedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetPromotionQuestionnaireSubmittedEvent: {Id}", @event.Id);
            var tpqResp = _mapper.Map<Domain.Entities.PQResponse>(@event);

            try
            {
                await _pqResponseRepository.Create(tpqResp);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), "TargetPromotionQuestionnaireSubmittedEvent Error creating PQResponse", ex);
            }
        }

        public async Task OnEvent(TargetPromotionQuestionnaireUpdatedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetPromotionQuestionnaireUpdatedEvent: {Id}", @event.Id);

            var existingTpqR = await _pqResponseRepository.ReadById(@event.Id) ??
             throw new EventHandlerException(nameof(EventHandler), $"TargetPromotionQuestionnaireUpdatedEvent Error updating {@event.Id}", new Exception("TargetPromotionQuestionnaire not found"));

            var tpqResp = _mapper.Map<Domain.Entities.PQResponse>(@event);
            tpqResp.DateCreated = existingTpqR.DateCreated;
            tpqResp.IsModified = true;

            try
            {
                await _pqResponseRepository.Update(tpqResp);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetPromotionQuestionnaireUpdatedEvent Error updating {@event.Id}", ex);
            }
        }


        public async Task OnEvent(TargetPromotionQuestionnaireDeletedEvent @event)
        {
            _logger.LogInformation("OnEvent: TargetPromotionQuestionnaireDeletedEvent: {Id}", @event.Id);
            var existingTpqR = await _pqResponseRepository.ReadById(@event.Id) ??
             throw new EventHandlerException(nameof(EventHandler), $"TargetPromotionQuestionnaireDeletedEvent Error deleting {@event.Id}", new Exception("TargetPromotionQuestionnaire not found"));

            try
            {
                await _pqResponseRepository.Delete(@event.Id);
            }
            catch (RepositoryException ex)
            {
                throw new EventHandlerException(nameof(EventHandler), $"TargetPromotionQuestionnaireDeletedEvent Error deleting {@event.Id}", ex);
            }
        }
    }
}