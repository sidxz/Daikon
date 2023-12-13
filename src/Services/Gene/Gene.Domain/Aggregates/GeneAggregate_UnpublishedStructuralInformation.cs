
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, UnpublishedStructuralInformation> _unpublishedStructuralInformation = [];

        /* Add UnpublishedStructuralInformation */
        public void AddUnpublishedStructuralInformation(UnpublishedStructuralInformation unpublishedStructuralInformation)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(unpublishedStructuralInformation.Organization))
            {
                throw new InvalidOperationException($" The value of unpublishedStructuralInformation organization cannot be null or whitespace");
            }


            RaiseEvent(new GeneUnpublishedStructuralInformationAddedEvent
            {
                Id = _id,
                GeneId = _id,
                UnpublishedStructuralInformationId = unpublishedStructuralInformation.UnpublishedStructuralInformationId,
                Organization = unpublishedStructuralInformation.Organization,
                Method = unpublishedStructuralInformation.Method,
                Resolution = unpublishedStructuralInformation.Resolution,
                Ligands = unpublishedStructuralInformation.Ligands,
                Researcher = unpublishedStructuralInformation.Researcher,
                Reference = unpublishedStructuralInformation.Reference,
                Notes = unpublishedStructuralInformation.Notes,
                URL = unpublishedStructuralInformation.URL,
                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(GeneUnpublishedStructuralInformationAddedEvent @event)
        {
            _unpublishedStructuralInformation.Add(@event.UnpublishedStructuralInformationId, new UnpublishedStructuralInformation
            {
                UnpublishedStructuralInformationId = @event.UnpublishedStructuralInformationId,
                Organization = @event.Organization,
                Method = @event.Method,
                Resolution = @event.Resolution,
                Ligands = @event.Ligands,
                Researcher = @event.Researcher,
                Reference = @event.Reference,
                Notes = @event.Notes,
                URL = @event.URL
            });
        }

        /* Update UnpublishedStructuralInformation */
        public void UpdateUnpublishedStructuralInformation(UnpublishedStructuralInformation unpublishedStructuralInformation)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_unpublishedStructuralInformation.ContainsKey(unpublishedStructuralInformation.UnpublishedStructuralInformationId))
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation does not exist.");
            }
            if (string.IsNullOrWhiteSpace(unpublishedStructuralInformation.Organization))
            {
                throw new InvalidOperationException($" The value of unpublishedStructuralInformation organization cannot be null or whitespace");
            }

            RaiseEvent(new GeneUnpublishedStructuralInformationUpdatedEvent
            {
                Id = _id,
                GeneId = _id,
                UnpublishedStructuralInformationId = unpublishedStructuralInformation.UnpublishedStructuralInformationId,
                Organization = unpublishedStructuralInformation.Organization,
                Method = unpublishedStructuralInformation.Method,
                Resolution = unpublishedStructuralInformation.Resolution,
                Ligands = unpublishedStructuralInformation.Ligands,
                Researcher = unpublishedStructuralInformation.Researcher,
                Reference = unpublishedStructuralInformation.Reference,
                Notes = unpublishedStructuralInformation.Notes,
                URL = unpublishedStructuralInformation.URL,
                DateUpdated = DateTime.UtcNow
            });
        }

        public void Apply(GeneUnpublishedStructuralInformationUpdatedEvent @event)
        {
            _id = @event.Id;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId] = new UnpublishedStructuralInformation
            {
                UnpublishedStructuralInformationId = @event.UnpublishedStructuralInformationId,
                Organization = @event.Organization,
                Method = @event.Method,
                Resolution = @event.Resolution,
                Ligands = @event.Ligands,
                Researcher = @event.Researcher,
                Reference = @event.Reference,
                Notes = @event.Notes,
                URL = @event.URL
                
            };
        }

        /* Delete UnpublishedStructuralInformation */
        public void DeleteUnpublishedStructuralInformation(UnpublishedStructuralInformation unpublishedStructuralInformation)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_unpublishedStructuralInformation.ContainsKey(unpublishedStructuralInformation.UnpublishedStructuralInformationId))
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation does not exist.");
            }

            RaiseEvent(new GeneUnpublishedStructuralInformationDeletedEvent
            {
                Id = _id,
                GeneId = _id,
                UnpublishedStructuralInformationId = unpublishedStructuralInformation.UnpublishedStructuralInformationId

            });
        }

        public void Apply(GeneUnpublishedStructuralInformationDeletedEvent @event)
        {
            _id = @event.Id;
            _unpublishedStructuralInformation.Remove(@event.UnpublishedStructuralInformationId);
        }
    }
}