
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, Hypomorph> _hypomorphs = [];

        /* Add Hypomorph */
        public void AddHypomorph(GeneHypomorphAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.HypomorphId == Guid.Empty)
            {
                throw new InvalidOperationException("Hypomorph Id cannot be empty.");
            }
            if (_hypomorphs.ContainsKey(@event.HypomorphId))
            {
                throw new Exception("Hypomorph already exists.");
            }
            

            RaiseEvent(@event);
        }

        public void Apply(GeneHypomorphAddedEvent @event)
        {
            _hypomorphs.Add(@event.HypomorphId, new Hypomorph
            {
                HypomorphId = @event.HypomorphId,
                KnockdownStrain = @event.KnockdownStrain,
                Phenotype = @event.Phenotype,
                Notes = @event.Notes,
            });
        }

        /* Update Hypomorph */
        public void UpdateHypomorph(GeneHypomorphUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.HypomorphId == Guid.Empty)
            {
                throw new InvalidOperationException("Hypomorph Id cannot be empty.");
            }

            if (!_hypomorphs.ContainsKey(@event.HypomorphId))
            {
                throw new InvalidOperationException("Hypomorph does not exist.");
            }
            

            RaiseEvent(@event);
        }

        public void Apply(GeneHypomorphUpdatedEvent @event)
        {
            _hypomorphs[@event.HypomorphId].KnockdownStrain = @event.KnockdownStrain;
            _hypomorphs[@event.HypomorphId].Phenotype = @event.Phenotype;
            _hypomorphs[@event.HypomorphId].Notes = @event.Notes;
        }

        /* Delete Hypomorph */
        public void DeleteHypomorph(GeneHypomorphDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.HypomorphId == Guid.Empty)
            {
                throw new InvalidOperationException("Hypomorph Id cannot be empty.");
            }
            if (!_hypomorphs.ContainsKey(@event.HypomorphId))
            {
                throw new InvalidOperationException("Hypomorph does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneHypomorphDeletedEvent @event)
        {
            _hypomorphs.Remove(@event.HypomorphId);
        }
    }
}