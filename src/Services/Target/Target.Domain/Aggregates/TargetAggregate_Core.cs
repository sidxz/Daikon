
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Targets;

namespace Target.Domain.Aggregates
{
    public  class TargetAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Entities.Target target;

        public TargetAggregate()
        {

        }


        /* New Target */
        public TargetAggregate(Entities.Target target, IMapper mapper)
        {
            _active = true;
            _id = target.Id;
            _Name = target.Name;

            var targetCreatedEvent = mapper.Map<TargetCreatedEvent>(target);

            RaiseEvent(targetCreatedEvent);
        }

        public void Apply(TargetCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
        }

        /* Update Target */
        public void UpdateTarget(Entities.Target target, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            var targetUpdatedEvent = mapper.Map<TargetUpdatedEvent>(target);
            targetUpdatedEvent.Id = target.Id;
            targetUpdatedEvent.Name = target.Name;

            RaiseEvent(targetUpdatedEvent);
        }

        public void Apply(TargetUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Update Target Associated Genes */
        public void UpdateTargetAssociatedGenes(Dictionary<string, string> associatedGenes, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            var targetAssociatedGenesUpdatedEvent = new TargetAssociatedGenesUpdatedEvent()
            {
                Id = target.Id,
                Name = _Name,
                AssociatedGenes = associatedGenes
            };
            targetAssociatedGenesUpdatedEvent.Id = target.Id;
            targetAssociatedGenesUpdatedEvent.Name = target.Name;

            RaiseEvent(targetAssociatedGenesUpdatedEvent);
        }

        public void Apply(TargetAssociatedGenesUpdatedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
        }

        /* Delete Target */
        public void DeleteTarget(Entities.Target target, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is already deleted.");
            }

            var targetDeletedEvent = new TargetDeletedEvent()
            {
                Id = target.Id,
                Name = _Name
            };
            
            RaiseEvent(targetDeletedEvent);
        }

        public void Apply(TargetDeletedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _active = false;
        }
    }
}