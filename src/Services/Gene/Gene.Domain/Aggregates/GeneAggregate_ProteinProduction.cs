
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, ProteinProduction> _proteinProduction = [];

        /* Add Protein Production */
        public void AddProteinProduction(GeneProteinProductionAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ProteinProductionId == Guid.Empty)
            {
                throw new InvalidOperationException("ProteinProduction Id cannot be empty.");
            }
            if (_proteinProduction.ContainsKey(@event.ProteinProductionId))
            {
                throw new Exception("ProteinProduction already exists.");
            }
            if (string.IsNullOrWhiteSpace(@event.Production))
            {
                throw new InvalidOperationException($"The value of production cannot be null or whitespace");
            }

            RaiseEvent(@event);
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
        public void UpdateProteinProduction(GeneProteinProductionUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ProteinProductionId == Guid.Empty)
            {
                throw new InvalidOperationException("ProteinProduction Id cannot be empty.");
            }
            if (!_proteinProduction.ContainsKey(@event.ProteinProductionId))
            {
                throw new InvalidOperationException("Protein production does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.Production))
            {
                throw new InvalidOperationException($" The value of production cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }
        

        public void Apply(GeneProteinProductionUpdatedEvent @event)
        {
            _proteinProduction[@event.ProteinProductionId].Production = @event.Production;
            _proteinProduction[@event.ProteinProductionId].Method = @event.Method;
            _proteinProduction[@event.ProteinProductionId].Purity = @event.Purity;
            _proteinProduction[@event.ProteinProductionId].DateProduced = @event.DateProduced;
            _proteinProduction[@event.ProteinProductionId].PMID = @event.PMID;
            _proteinProduction[@event.ProteinProductionId].Notes = @event.Notes;
            _proteinProduction[@event.ProteinProductionId].URL = @event.URL;
        }

        /* Delete Protein Production */
        public void DeleteProteinProduction(GeneProteinProductionDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.ProteinProductionId == Guid.Empty)
            {
                throw new InvalidOperationException("ProteinProduction Id cannot be empty.");
            }
            if (!_proteinProduction.ContainsKey(@event.ProteinProductionId))
            {
                throw new InvalidOperationException("Protein production does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneProteinProductionDeletedEvent @event)
        {
            _proteinProduction.Remove(@event.ProteinProductionId);
        }
    }
}