
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, Hypomorph> _hypomorphs = [];

        /* Add Hypomorph */
        public void AddHypomorph(Hypomorph hypomorph)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(hypomorph.KnockdownStrain))
            {
                throw new InvalidOperationException($" The value of hypomorph cannot be null or whitespace");
            }


            RaiseEvent(new GeneHypomorphAddedEvent
            {
                Id = _id,
                GeneId = _id,
                HypomorphId = hypomorph.HypomorphId,
                KnockdownStrain = hypomorph.KnockdownStrain,
                Phenotype = hypomorph.Phenotype,
                Notes = hypomorph.Notes,
                DateCreated = DateTime.UtcNow
            });
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
        public void UpdateHypomorph(Hypomorph hypomorph)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_hypomorphs.ContainsKey(hypomorph.HypomorphId))
            {
                throw new InvalidOperationException("Hypomorph does not exist.");
            }
            if (string.IsNullOrWhiteSpace(hypomorph.KnockdownStrain))
            {
                throw new InvalidOperationException($" The value of hypomorph classification cannot be null or whitespace");
            }

            RaiseEvent(new GeneHypomorphUpdatedEvent
            {
                Id = _id,
                GeneId = _id,
                HypomorphId = hypomorph.HypomorphId,
                KnockdownStrain = hypomorph.KnockdownStrain,
                Phenotype = hypomorph.Phenotype,
                Notes = hypomorph.Notes,
                DateUpdated = DateTime.UtcNow
            });
        }

        public void Apply(GeneHypomorphUpdatedEvent @event)
        {
            _id = @event.Id;
            _hypomorphs[@event.HypomorphId] = new Hypomorph
            {
                HypomorphId = @event.HypomorphId,
                KnockdownStrain = @event.KnockdownStrain,
                Phenotype = @event.Phenotype,
                Notes = @event.Notes,
            };
        }

        /* Delete Hypomorph */
        public void DeleteHypomorph(Hypomorph hypomorph)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_hypomorphs.ContainsKey(hypomorph.HypomorphId))
            {
                throw new InvalidOperationException("Hypomorph does not exist.");
            }

            RaiseEvent(new GeneHypomorphDeletedEvent
            {
                Id = _id,
                GeneId = _id,
                HypomorphId = hypomorph.HypomorphId

            });
        }

        public void Apply(GeneHypomorphDeletedEvent @event)
        {
            _id = @event.Id;
            _hypomorphs.Remove(@event.HypomorphId);
        }
    }
}