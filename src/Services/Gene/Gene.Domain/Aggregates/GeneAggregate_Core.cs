
using CQRS.Core.Domain;
using Daikon.Events.Gene;


namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate : AggregateRoot
    {

        private bool _active;
        private string _AccessionNumber;
        public string _Name { get; set; }

        public GeneAggregate()
        {

        }

        /* New Gene */

        public GeneAggregate(GeneCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _AccessionNumber = @event.AccessionNumber;
            _Name = @event.Name;

            RaiseEvent(@event);
        }

        public void Apply(GeneCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _AccessionNumber = @event.AccessionNumber;
            _Name = @event.Name;
        }

        /* Update Gene */

        public void UpdateGene(GeneUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneUpdatedEvent @event)
        {
            _Name = @event.Name;
        }

        /* Delete Gene */
        public void DeleteGene(GeneDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is already deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneDeletedEvent @event)
        {
            _active = false;
        }
    }
}