
using CQRS.Core.Domain;
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public class GeneAggregate : AggregateRoot
    {

        private bool _active;
        private string _AccessionNumber;
        private readonly Dictionary<Guid, Essentiality> _essentialities = [];
        public GeneAggregate()
        {

        }

        /* New Gene */

        public GeneAggregate(Entities.Gene gene)
        {
            _active = true;
            _id = gene.Id;
            _AccessionNumber = gene.AccessionNumber;

            RaiseEvent(new GeneCreatedEvent
            {
                Name = gene.Name,
                Id = gene.Id,

                StrainId = gene.StrainId,
                AccessionNumber = gene.AccessionNumber,
                Function = gene.Function,
                Product = gene.Product,
                FunctionalCategory = gene.FunctionalCategory,

                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(GeneCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _AccessionNumber = @event.AccessionNumber;
        }

        /* Update Gene */

        public void UpdateGene(Entities.Gene gene)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            RaiseEvent(new GeneUpdatedEvent
            {
                Id = gene.Id,
                StrainId = gene.StrainId,
                Name = gene.Name,
                AccessionNumber = gene.AccessionNumber,
                Function = gene.Function,
                Product = gene.Product,
                FunctionalCategory = gene.FunctionalCategory,

            });
        }

        public void Apply(GeneUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete Gene */
        public void DeleteGene(Entities.Gene gene)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is already deleted.");
            }

            RaiseEvent(new GeneDeletedEvent
            {
                Id = gene.Id,
                AccessionNumber = gene.AccessionNumber
            });
        }

        public void Apply(GeneDeletedEvent @event)
        {
            _id = @event.Id;
            _AccessionNumber = @event.AccessionNumber;
            _active = false;
        }



        /* Add Essentiality */
        public void AddEssentiality(Essentiality essentiality)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(essentiality.Classification))
            {
                throw new InvalidOperationException($" The value of essentiality classification cannot be null or whitespace");
            }


            RaiseEvent(new GeneEssentialityAddedEvent
            {
                Id = _id,
                EssentialityId = essentiality.EssentialityId,
                Classification = essentiality.Classification,
                Condition = essentiality.Condition,
                Method = essentiality.Method,
                Reference = essentiality.Reference,
                Note = essentiality.Note,
                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(GeneEssentialityAddedEvent @event)
        {
            _essentialities.Add(@event.EssentialityId, new Essentiality
            {
                EssentialityId = @event.EssentialityId,
                Classification = @event.Classification,
                Condition = @event.Condition,
                Method = @event.Method,
                Reference = @event.Reference,
                Note = @event.Note
            });
        }

        /* Update Essentiality */
        public void UpdateEssentiality(Essentiality essentiality)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_essentialities.ContainsKey(essentiality.EssentialityId))
            {
                throw new InvalidOperationException("Essentiality does not exist.");
            }
            if (string.IsNullOrWhiteSpace(essentiality.Classification))
            {
                throw new InvalidOperationException($" The value of essentiality classification cannot be null or whitespace");
            }

            RaiseEvent(new GeneEssentialityUpdatedEvent
            {
                Id = _id,
                EssentialityId = essentiality.EssentialityId,
                Classification = essentiality.Classification,
                Condition = essentiality.Condition,
                Method = essentiality.Method,
                Reference = essentiality.Reference,
                Note = essentiality.Note,
                DateUpdated = DateTime.UtcNow
            });
        }

        public void Apply(GeneEssentialityUpdatedEvent @event)
        {
            _id = @event.Id;
            _essentialities[@event.EssentialityId] = new Essentiality
            {
                EssentialityId = @event.EssentialityId,
                Classification = @event.Classification,
                Condition = @event.Condition,
                Method = @event.Method,
                Reference = @event.Reference,
                Note = @event.Note
            };
        }

        /* Delete Essentiality */
        public void DeleteEssentiality(Essentiality essentiality)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_essentialities.ContainsKey(essentiality.EssentialityId))
            {
                throw new InvalidOperationException("Essentiality does not exist.");
            }

            RaiseEvent(new GeneEssentialityDeletedEvent
            {
                Id = _id,
                EssentialityId = essentiality.EssentialityId

            });
        }

        public void Apply(GeneEssentialityDeletedEvent @event)
        {
            _id = @event.Id;
            _essentialities.Remove(@event.EssentialityId);
        }
    }
}