
using CQRS.Core.Domain;
using Daikon.Events.Gene;
using Daikon.Events.Strains;
using Gene.Domain.Entities;

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

        public StrainAggregate(Strain strain)
        {
            _active = true;
            _id = strain.Id;
            _Name = strain.Name;

            RaiseEvent(new StrainCreatedEvent
            {
                Name = strain.Name,
                Id = strain.Id,

                Organism = strain.Organism,
                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(StrainCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
        }

        /* Update Strain */

        public void UpdateStrain(Strain strain)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This strain is deleted.");
            }

            RaiseEvent(new StrainUpdatedEvent
            {
                Id = strain.Id,
                Name = strain.Name,
                Organism = strain.Organism,
            });
        }

        public void Apply(StrainUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete Strain */
        public void DeleteGene(Strain strain)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This strain is already deleted.");
            }

            RaiseEvent(new StrainDeletedEvent
            {
                Id = strain.Id,
                Name = strain.Name
            });
        }

        public void Apply(StrainDeletedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _active = false;
        }
    }
}