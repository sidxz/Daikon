
using CQRS.Core.Domain;
using Daikon.Events.Gene;

namespace Gene.Domain.Aggregates
{
    public class GeneAggregate : AggregateRoot
    {

        private bool _active;
        private string _AccessionNumber;
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
    }
}