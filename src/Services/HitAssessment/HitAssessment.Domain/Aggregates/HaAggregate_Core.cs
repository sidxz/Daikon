using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.Events.HitAssessment;

namespace HitAssessment.Domain.Aggregates
{
    public partial class HaAggregate: AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Guid _compoundId;
        private Guid _hitId;
        private Dictionary<string, string> _associatedHits { get; set; }

        public HaAggregate()
        {

        }

        /* New Hit Assessment */
        public HaAggregate(HaCreatedEvent haCreatedEvent)
        {
            _active = true;
            _id = haCreatedEvent.Id;
            _Name = haCreatedEvent.Name;
            _compoundId = haCreatedEvent.CompoundId;
            _hitId = haCreatedEvent.HitId;


            RaiseEvent(haCreatedEvent);
        }

        public void Apply(HaCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _compoundId = @event.CompoundId;
            _hitId = @event.HitId;

        }

        /* Update Hit Assessment */
        public void UpdateHa(HaUpdatedEvent haUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }

            // HaUpdatedEvent doesn't allow name or HitId to be changed.
            haUpdatedEvent.Name = _Name;
            haUpdatedEvent.CompoundId = _compoundId;
            haUpdatedEvent.HitId = _hitId;

            RaiseEvent(haUpdatedEvent);
        }

        public void Apply(HaUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete Hit Assessment */
        public void DeleteHa(HaDeletedEvent haDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is already deleted.");
            }

            RaiseEvent(haDeletedEvent);
        }
        public void Apply(HaDeletedEvent @event)
        {
            _active = false;
        }
    }
}