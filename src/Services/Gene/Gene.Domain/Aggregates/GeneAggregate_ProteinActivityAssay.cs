
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, ProteinActivityAssay> _proteinActivityAssay = [];

        /* Add ProteinActivityAssay */
        public void AddProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(proteinActivityAssay.Assay))
            {
                throw new InvalidOperationException($" The value of proteinActivityAssay Assay cannot be null or whitespace");
            }


            RaiseEvent(new GeneProteinActivityAssayAddedEvent
            {
                Id = _id,
                GeneId = _id,
                ProteinActivityAssayId = proteinActivityAssay.ProteinActivityAssayId,
                Assay = proteinActivityAssay.Assay,
                Method = proteinActivityAssay.Method,
                Throughput = proteinActivityAssay.Throughput,
                PMID = proteinActivityAssay.PMID,
                Reference = proteinActivityAssay.Reference,
                URL = proteinActivityAssay.URL,
                DateCreated = DateTime.UtcNow
            });
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
        public void UpdateProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_proteinActivityAssay.ContainsKey(proteinActivityAssay.ProteinActivityAssayId))
            {
                throw new InvalidOperationException("ProteinActivityAssay does not exist.");
            }
            if (string.IsNullOrWhiteSpace(proteinActivityAssay.Assay))
            {
                throw new InvalidOperationException($" The value of proteinActivityAssay Assay cannot be null or whitespace");
            }

            RaiseEvent(new GeneProteinActivityAssayUpdatedEvent
            {
                Id = _id,
                GeneId = _id,
                ProteinActivityAssayId = proteinActivityAssay.ProteinActivityAssayId,
                Assay = proteinActivityAssay.Assay,
                Method = proteinActivityAssay.Method,
                Throughput = proteinActivityAssay.Throughput,
                PMID = proteinActivityAssay.PMID,
                Reference = proteinActivityAssay.Reference,
                URL = proteinActivityAssay.URL,
                DateUpdated = DateTime.UtcNow
            });
        }

        public void Apply(GeneProteinActivityAssayUpdatedEvent @event)
        {
            _id = @event.Id;
            _proteinActivityAssay[@event.ProteinActivityAssayId] = new ProteinActivityAssay
            {
                ProteinActivityAssayId = @event.ProteinActivityAssayId,
                Assay = @event.Assay,
                Method = @event.Method,
                Throughput = @event.Throughput,
                PMID = @event.PMID,
                Reference = @event.Reference,
                URL = @event.URL
            };
        }

        /* Delete ProteinActivityAssay */
        public void DeleteProteinActivityAssay(ProteinActivityAssay proteinActivityAssay)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_proteinActivityAssay.ContainsKey(proteinActivityAssay.ProteinActivityAssayId))
            {
                throw new InvalidOperationException("ProteinActivityAssay does not exist.");
            }

            RaiseEvent(new GeneProteinActivityAssayDeletedEvent
            {
                Id = _id,
                GeneId = _id,
                ProteinActivityAssayId = proteinActivityAssay.ProteinActivityAssayId

            });
        }

        public void Apply(GeneProteinActivityAssayDeletedEvent @event)
        {
            _id = @event.Id;
            _proteinActivityAssay.Remove(@event.ProteinActivityAssayId);
        }
    }
}