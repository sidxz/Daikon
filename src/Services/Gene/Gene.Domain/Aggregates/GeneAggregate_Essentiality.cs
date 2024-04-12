
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, Essentiality> _essentialities = [];

        /* Add Essentiality */
        public void AddEssentiality(GeneEssentialityAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.EssentialityId == Guid.Empty)
            {
                throw new InvalidOperationException("Essentiality Id cannot be empty.");
            }
            if (_essentialities.ContainsKey(@event.EssentialityId))
            {
                throw new Exception("Essentiality already exists.");
            }
            if (string.IsNullOrWhiteSpace(@event.Classification))
            {
                throw new InvalidOperationException($" The value of essentiality classification cannot be null or whitespace");
            }
            RaiseEvent(@event);
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
        public void UpdateEssentiality(GeneEssentialityUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.EssentialityId == Guid.Empty)
            {
                throw new InvalidOperationException("Essentiality Id cannot be empty.");
            }
            if (!_essentialities.ContainsKey(@event.EssentialityId))
            {
                throw new InvalidOperationException("Essentiality does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.Classification))
            {
                throw new InvalidOperationException($" The value of essentiality classification cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneEssentialityUpdatedEvent @event)
        {
            _essentialities[@event.EssentialityId].Classification = @event.Classification;
            _essentialities[@event.EssentialityId].Condition = @event.Condition;
            _essentialities[@event.EssentialityId].Method = @event.Method;
            _essentialities[@event.EssentialityId].Reference = @event.Reference;
            _essentialities[@event.EssentialityId].Note = @event.Note;
        }

        /* Delete Essentiality */
        public void DeleteEssentiality(GeneEssentialityDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.EssentialityId == Guid.Empty)
            {
                throw new InvalidOperationException("Essentiality Id cannot be empty.");
            }
            if (!_essentialities.ContainsKey(@event.EssentialityId))
            {
                throw new InvalidOperationException("Essentiality does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneEssentialityDeletedEvent @event)
        {
            _essentialities.Remove(@event.EssentialityId);
        }
    }
}