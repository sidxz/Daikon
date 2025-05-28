
using Daikon.Events.Gene;
using Daikon.EventStore.Aggregate;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, GeneExpansionProp> _expansionProps = [];

        public void AddExpansionProp(GeneExpansionPropAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene has been deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ExpansionPropId == Guid.Empty)
            {
                throw new InvalidOperationException("Expansion Prop Id cannot be empty.");
            }

            if (_expansionProps.ContainsKey(@event.ExpansionPropId))
            {
                throw new Exception("Expansion Props already exists.");
            }
            RaiseEvent(@event);
        }

        public void Apply(GeneExpansionPropAddedEvent @event)
        {
            _expansionProps.Add(@event.ExpansionPropId, new GeneExpansionProp()
            {
                GeneId = @event.Id,
                ExpansionType = @event.ExpansionType,
                ExpansionValue = @event.ExpansionValue
            });
        }

        public void UpdateExpansionProp(GeneExpansionPropUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene has been deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ExpansionPropId == Guid.Empty)
            {
                throw new InvalidOperationException("Expansion Prop Id cannot be empty.");
            }

            if (!_expansionProps.ContainsKey(@event.ExpansionPropId))
            {
                throw new Exception("Expansion Props does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneExpansionPropUpdatedEvent @event)
        {
            _expansionProps[@event.ExpansionPropId].ExpansionValue = @event.ExpansionValue;
        }

        public void DeleteExpansionProp(GeneExpansionPropDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene has been deleted.");
            }
            
            if (@event.ExpansionPropId == Guid.Empty)
            {
                throw new InvalidOperationException("Expansion Prop Id cannot be empty.");
            }

            if (!_expansionProps.ContainsKey(@event.ExpansionPropId))
            {
                throw new Exception("Expansion Props does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneExpansionPropDeletedEvent @event)
        {
            _expansionProps.Remove(@event.ExpansionPropId);
        }
    }
}