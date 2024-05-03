using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.Events.HitAssessment;

namespace HitAssessment.Domain.Aggregates
{
    public partial class HaAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Guid _compoundId;
        private Guid _hitId;
        private Dictionary<string, string> _associatedHits { get; set; } // MoleculeId, HitId

        public HaAggregate()
        {

        }

        /* New Hit Assessment */
        public HaAggregate(HaCreatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.Name == null)
            {
                throw new InvalidOperationException("Name cannot be empty.");
            }

            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _compoundId = @event.CompoundId;
            _hitId = @event.HitId;

            RaiseEvent(@event);
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
        public void UpdateHa(HaUpdatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is deleted.");
            }

            // HaUpdatedEvent doesn't allow name or HitId to be changed.
            @event.Name = _Name;
            @event.CompoundId = _compoundId;
            @event.HitId = _hitId;

            RaiseEvent(@event);
        }

        public void Apply(HaUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete Hit Assessment */
        public void DeleteHa(HaDeletedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }

            if (!_active)
            {
                throw new InvalidOperationException("This Hit Assessment is already deleted.");
            }

            RaiseEvent(@event);
        }
        public void Apply(HaDeletedEvent @event)
        {
            _active = false;
        }
    }
}