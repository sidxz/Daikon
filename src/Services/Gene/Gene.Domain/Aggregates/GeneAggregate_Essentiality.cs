
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, Essentiality> _essentialities = [];

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
                GeneId = _id,
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
                GeneId = _id,
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
                GeneId = _id,
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