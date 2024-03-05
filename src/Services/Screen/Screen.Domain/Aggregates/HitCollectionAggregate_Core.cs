
using Amazon.Runtime.Internal.Util;
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Screens;
using Microsoft.Extensions.Logging;
using Screen.Domain.Entities;

namespace Screen.Domain.Aggregates
{
    public partial class HitCollectionAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Guid _ScreenId;

        public HitCollectionAggregate()
        {
        }

        /* Add HitCollection */
        public HitCollectionAggregate(HitCollectionCreatedEvent hitCollectionCreatedEvent)
        {
            _active = true;
            _id = hitCollectionCreatedEvent.Id;
            _Name = hitCollectionCreatedEvent.Name;
            _ScreenId = hitCollectionCreatedEvent.ScreenId;

            RaiseEvent(hitCollectionCreatedEvent);
        }

        public void Apply(HitCollectionCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
            _ScreenId = @event.ScreenId;
        }

        /* Update HitCollection */
        public void UpdateHitCollection(HitCollectionUpdatedEvent hitCollectionUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is deleted.");
            }
            // HitCollectionUpdatedEvent doesn't allow name or associated screen to be changed.
            hitCollectionUpdatedEvent.Id = _id;
            hitCollectionUpdatedEvent.ScreenId = _ScreenId;
            hitCollectionUpdatedEvent.Name = _Name;

            RaiseEvent(hitCollectionUpdatedEvent);
        }

        public void Apply(HitCollectionUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete HitCollection */
        public void DeleteHitCollection(HitCollectionDeletedEvent hitCollectionDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is already deleted.");
            }

            RaiseEvent(hitCollectionDeletedEvent);
        }

        public void Apply(HitCollectionDeletedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }

        /* Rename HitCollection */

        public void RenameHitCollection(HitCollectionRenamedEvent hitCollectionRenamedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Unable to rename the hit collection as it has been deleted.");
            }
            if (hitCollectionRenamedEvent.Name == _Name)
            {
                throw new InvalidOperationException("Unable to rename the hit collection as it already has the specified name.");
            }

            RaiseEvent(hitCollectionRenamedEvent);
        }

        public void Apply(HitCollectionRenamedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
        }

        /* Update HitCollection Associated Screen */
        public void UpdateHitCollectionAssociatedScreen(HitCollectionAssociatedScreenUpdatedEvent hitCollectionAssociatedScreenUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection has already been deleted.");
            }

            if (_ScreenId == hitCollectionAssociatedScreenUpdatedEvent.ScreenId)
            {
                throw new InvalidOperationException("The associated screen already belongs to the specified screen.");
            }
            RaiseEvent(hitCollectionAssociatedScreenUpdatedEvent);
        }

        public void Apply(HitCollectionAssociatedScreenUpdatedEvent @event)
        {
            _id = @event.Id;
            _ScreenId = @event.ScreenId;
        }
    }
}