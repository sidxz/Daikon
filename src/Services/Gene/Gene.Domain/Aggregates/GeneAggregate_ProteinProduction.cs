
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, ProteinProduction> _proteinProduction = [];

        /* Add Protein Production */
        public void AddProteinProduction(ProteinProduction proteinProduction)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(proteinProduction.Production))
            {
                throw new InvalidOperationException($" The value of production cannot be null or whitespace");
            }


            RaiseEvent(new GeneProteinProductionAddedEvent
            {
                Id = _id,
                GeneId = _id,
                ProteinProductionId = proteinProduction.ProteinProductionId,
                Production = proteinProduction.Production,
                Method = proteinProduction.Method,
                Purity = proteinProduction.Purity,
                DateProduced = proteinProduction.DateProduced,
                PMID = proteinProduction.PMID,
                Notes = proteinProduction.Notes,
                URL = proteinProduction.URL,
                DateCreated = DateTime.UtcNow
            });
            
        }

        public void Apply(GeneProteinProductionAddedEvent @event)
        {
            _proteinProduction.Add(@event.ProteinProductionId, new ProteinProduction
            {
                ProteinProductionId = @event.ProteinProductionId,
                Production = @event.Production,
                Method = @event.Method,
                Purity = @event.Purity,
                DateProduced = @event.DateProduced,
                PMID = @event.PMID,
                Notes = @event.Notes,
                URL = @event.URL
            });
        }

        /* Update Protein Production */
        public void UpdateProteinProduction(ProteinProduction proteinProduction)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_proteinProduction.ContainsKey(proteinProduction.ProteinProductionId))
            {
                throw new InvalidOperationException("Protein production does not exist.");
            }
            if (string.IsNullOrWhiteSpace(proteinProduction.Production))
            {
                throw new InvalidOperationException($" The value of production cannot be null or whitespace");
            }

            RaiseEvent(new GeneProteinProductionUpdatedEvent
            {
                Id = _id,
                GeneId = _id,
                ProteinProductionId = proteinProduction.ProteinProductionId,
                Production = proteinProduction.Production,
                Method = proteinProduction.Method,
                Purity = proteinProduction.Purity,
                DateProduced = proteinProduction.DateProduced,
                PMID = proteinProduction.PMID,
                Notes = proteinProduction.Notes,
                URL = proteinProduction.URL,
                DateUpdated = DateTime.UtcNow
            });
            
        }

        public void Apply(GeneProteinProductionUpdatedEvent @event)
        {
            _id = @event.Id;
            _proteinProduction[@event.ProteinProductionId] = new ProteinProduction
            {
                ProteinProductionId = @event.ProteinProductionId,
                Production = @event.Production,
                Method = @event.Method,
                Purity = @event.Purity,
                DateProduced = @event.DateProduced,
                PMID = @event.PMID,
                Notes = @event.Notes,
                URL = @event.URL
            };
        }

        /* Delete Protein Production */
        public void DeleteProteinProduction(ProteinProduction proteinProduction)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_proteinProduction.ContainsKey(proteinProduction.ProteinProductionId))
            {
                throw new InvalidOperationException("Protein production does not exist.");
            }
            

            RaiseEvent(new GeneProteinProductionDeletedEvent
            {
                Id = _id,
                GeneId = _id,
                ProteinProductionId = proteinProduction.ProteinProductionId

            });
        }

        public void Apply(GeneProteinProductionDeletedEvent @event)
        {
            _id = @event.Id;
            _proteinProduction.Remove(@event.ProteinProductionId);
        }
    }
}