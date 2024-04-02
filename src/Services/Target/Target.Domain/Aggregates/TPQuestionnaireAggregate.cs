
using CQRS.Core.Domain;
using Daikon.Events.Targets;

namespace Target.Domain.Aggregates
{
    public class TPQuestionnaireAggregate : AggregateRoot
    {
        private bool _active;
        private string _RequestedTargetName;
        private string _ApprovedTargetName;
        private bool _isVerified;

        public TPQuestionnaireAggregate()
        {

        }


        /* New PQResponse */
        public TPQuestionnaireAggregate(TargetPromotionQuestionnaireSubmittedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _RequestedTargetName = @event.RequestedTargetName;
            _ApprovedTargetName = null;
            _isVerified = false;

            RaiseEvent(@event);
        }

        public void Apply(TargetPromotionQuestionnaireSubmittedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _RequestedTargetName = @event.RequestedTargetName;
            _ApprovedTargetName = null;
            _isVerified = @event.IsVerified ?? false;
        }

        /* Update PQResponse */
        public void UpdatePQResponse(TargetPromotionQuestionnaireUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This PQResponse is deleted.");
            }
            @event.Id = _id;
            _RequestedTargetName = @event.RequestedTargetName;
            _ApprovedTargetName = @event.ApprovedTargetName ?? null;
            _isVerified = @event.IsVerified ?? false;

            RaiseEvent(@event);
        }

        public void Apply(TargetPromotionQuestionnaireUpdatedEvent @event)
        {
            _id = @event.Id;
            _RequestedTargetName = @event.RequestedTargetName;
            _ApprovedTargetName = @event.ApprovedTargetName ?? null;
            _isVerified = @event.IsVerified ?? false;
        }

        /* Delete PQResponse */
        public void DeletePQResponse(TargetPromotionQuestionnaireDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This PQResponse is already deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(TargetPromotionQuestionnaireDeletedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}