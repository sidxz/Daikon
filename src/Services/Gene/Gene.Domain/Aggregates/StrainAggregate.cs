
using Daikon.Events.Strains;
using Daikon.EventStore.Aggregate;

namespace Gene.Domain.Aggregates
{
    public class StrainAggregate : AggregateRoot
    {

        private bool _active;
        private string _Name;
        public StrainAggregate()
        {

        }

        /* New Strain */

        public StrainAggregate(StrainCreatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.Name == null)
            {
                throw new InvalidOperationException("Strain Name cannot be empty.");
            }
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;

            RaiseEvent(@event);
        }

        public void Apply(StrainCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
        }

        /* Update Strain */

        public void UpdateStrain(StrainUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This strain is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.Name == null)
            {
                throw new InvalidOperationException("Strain Name cannot be empty.");
            }

            RaiseEvent(@event);
        }


        public void Apply(StrainUpdatedEvent @event)
        {
            _Name = @event.Name;
        }

        /* Delete Strain */
        public void DeleteGene(StrainDeletedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }

            if (!_active)
            {
                throw new InvalidOperationException("This strain is already deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(StrainDeletedEvent @event)
        {
            _active = false;
        }
    }
}