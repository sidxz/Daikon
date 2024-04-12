
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, UnpublishedStructuralInformation> _unpublishedStructuralInformation = [];

        /* Add UnpublishedStructuralInformation */
        public void AddUnpublishedStructuralInformation(GeneUnpublishedStructuralInformationAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.UnpublishedStructuralInformationId == Guid.Empty)
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation Id cannot be empty.");
            }
            if (_unpublishedStructuralInformation.ContainsKey(@event.UnpublishedStructuralInformationId))
            {
                throw new Exception("UnpublishedStructuralInformation already exists.");
            }
            if (string.IsNullOrWhiteSpace(@event.Organization))
            {
                throw new InvalidOperationException($"The value of organization cannot be null or whitespace");
            }
            RaiseEvent(@event);
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
        public void UpdateUnpublishedStructuralInformation(GeneUnpublishedStructuralInformationUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.UnpublishedStructuralInformationId == Guid.Empty)
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation Id cannot be empty.");
            }
            if (!_unpublishedStructuralInformation.ContainsKey(@event.UnpublishedStructuralInformationId))
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.Organization))
            {
                throw new InvalidOperationException($" The value of unpublishedStructuralInformation organization cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }
        
        public void Apply(GeneUnpublishedStructuralInformationUpdatedEvent @event)
        {
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Organization = @event.Organization;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Method = @event.Method;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Resolution = @event.Resolution;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Ligands = @event.Ligands;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Researcher = @event.Researcher;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Reference = @event.Reference;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].Notes = @event.Notes;
            _unpublishedStructuralInformation[@event.UnpublishedStructuralInformationId].URL = @event.URL;
        }

        /* Delete UnpublishedStructuralInformation */
        public void DeleteUnpublishedStructuralInformation(GeneUnpublishedStructuralInformationDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.UnpublishedStructuralInformationId == Guid.Empty)
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation Id cannot be empty.");
            }
            if (!_unpublishedStructuralInformation.ContainsKey(@event.UnpublishedStructuralInformationId))
            {
                throw new InvalidOperationException("UnpublishedStructuralInformation does not exist.");
            }
            RaiseEvent(@event);
        }

        public void Apply(GeneUnpublishedStructuralInformationDeletedEvent @event)
        {
            _unpublishedStructuralInformation.Remove(@event.UnpublishedStructuralInformationId);
        }
    }
}