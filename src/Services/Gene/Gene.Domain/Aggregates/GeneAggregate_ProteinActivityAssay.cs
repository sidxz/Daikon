
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, ProteinActivityAssay> _proteinActivityAssay = [];

        /* Add ProteinActivityAssay */
        public void AddProteinActivityAssay(GeneProteinActivityAssayAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ProteinActivityAssayId == Guid.Empty)
            {
                throw new InvalidOperationException("ProteinActivityAssay Id cannot be empty.");
            }
            if (_proteinActivityAssay.ContainsKey(@event.ProteinActivityAssayId))
            {
                throw new Exception("ProteinActivityAssay already exists.");
            }
            if (string.IsNullOrWhiteSpace(@event.Assay))
            {
                throw new InvalidOperationException($" The value of proteinActivityAssay Assay cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneProteinActivityAssayAddedEvent @event)
        {
            _proteinActivityAssay.Add(@event.ProteinActivityAssayId, new ProteinActivityAssay
            {
                ProteinActivityAssayId = @event.ProteinActivityAssayId,
                Assay = @event.Assay,
                Method = @event.Method,
                Throughput = @event.Throughput,
                PMID = @event.PMID,
                Reference = @event.Reference,
                URL = @event.URL
            });
        }

        /* Update ProteinActivityAssay */
        public void UpdateProteinActivityAssay(GeneProteinActivityAssayUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (!_proteinActivityAssay.ContainsKey(@event.ProteinActivityAssayId))
            {
                throw new InvalidOperationException("ProteinActivityAssay does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.Assay))
            {
                throw new InvalidOperationException($" The value of Assay cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }



        public void Apply(GeneProteinActivityAssayUpdatedEvent @event)
        {
            _proteinActivityAssay[@event.ProteinActivityAssayId].Assay = @event.Assay;
            _proteinActivityAssay[@event.ProteinActivityAssayId].Method = @event.Method;
            _proteinActivityAssay[@event.ProteinActivityAssayId].Throughput = @event.Throughput;
            _proteinActivityAssay[@event.ProteinActivityAssayId].PMID = @event.PMID;
            _proteinActivityAssay[@event.ProteinActivityAssayId].Reference = @event.Reference;
            _proteinActivityAssay[@event.ProteinActivityAssayId].URL = @event.URL;
        }

        /* Delete ProteinActivityAssay */
        public void DeleteProteinActivityAssay(GeneProteinActivityAssayDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.ProteinActivityAssayId == Guid.Empty)
            {
                throw new InvalidOperationException("ProteinActivityAssay Id cannot be empty.");
            }
            if (!_proteinActivityAssay.ContainsKey(@event.ProteinActivityAssayId))
            {
                throw new InvalidOperationException("ProteinActivityAssay does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneProteinActivityAssayDeletedEvent @event)
        {
            _proteinActivityAssay.Remove(@event.ProteinActivityAssayId);
        }
    }
}