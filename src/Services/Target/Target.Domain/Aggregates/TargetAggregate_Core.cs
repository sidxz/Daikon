
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Targets;

namespace Target.Domain.Aggregates
{
    public class TargetAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Entities.Target target;

        public TargetAggregate()
        {

        }


        /* New Target */
        public TargetAggregate(TargetCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;

            RaiseEvent(@event);
        }

        public void Apply(TargetCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
        }

        /* Update Target */
        public void UpdateTarget(TargetUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            @event.Id = target.Id;
            @event.Name = target.Name;

            RaiseEvent(@event);
        }

        public void Apply(TargetUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Update Target Associated Genes */
        public void UpdateTargetAssociatedGenes(TargetAssociatedGenesUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            @event.Id = target.Id;
            @event.Name = target.Name;

            RaiseEvent(@event);
        }

        public void Apply(TargetAssociatedGenesUpdatedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
        }

        /* Delete Target */
        public void DeleteTarget(TargetDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is already deleted.");
            }
            
            RaiseEvent(@event);
        }

        public void Apply(TargetDeletedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _active = false;
        }
    }
}