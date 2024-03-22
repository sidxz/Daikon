
using CQRS.Core.Domain;
using Daikon.Events.Targets;
using CQRS.Core.Comparators;
namespace Target.Domain.Aggregates
{
    public class TargetAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        public Dictionary<string, string> _associatedGenes { get; set; }

        public TargetAggregate()
        {

        }


        /* New Target */
        public TargetAggregate(TargetCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _associatedGenes = @event.AssociatedGenes;

            RaiseEvent(@event);
        }

        public void Apply(TargetCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
            _associatedGenes = @event.AssociatedGenes;
        }

        /* Update Target */
        public void UpdateTarget(TargetUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            @event.Id = _id;
            @event.Name = _Name;
            @event.AssociatedGenes = _associatedGenes;

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

            if (_associatedGenes.DictionaryEqual(@event.AssociatedGenes))
            {
                throw new InvalidOperationException("Associated genes are not modified");
            }

            @event.Id = _id;
            @event.Name = _Name;

            RaiseEvent(@event);
        }

        public void Apply(TargetAssociatedGenesUpdatedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _associatedGenes = @event.AssociatedGenes;
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

        /* Target Rename Event */
        public void RenameTarget(TargetRenamedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }
            if (_Name == @event.Name)
            {
                throw new InvalidOperationException("Target name is not modified");
            }

            RaiseEvent(@event);
        }

        public void Apply(TargetRenamedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
        }

    }
}