
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, ResistanceMutation> _resistanceMutation = [];

        /* Add ResistanceMutation */
        public void AddResistanceMutation(ResistanceMutation resistanceMutation)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (string.IsNullOrWhiteSpace(resistanceMutation.Mutation))
            {
                throw new InvalidOperationException($" The value of resistanceMutation mutation cannot be null or whitespace");
            }


            RaiseEvent(new GeneResistanceMutationAddedEvent
            {
                Id = _id,
                GeneId = _id,
                ResistanceMutationId = resistanceMutation.ResistanceMutationId,
                Mutation = resistanceMutation.Mutation,
                Isolate = resistanceMutation.Isolate,
                ParentStrain = resistanceMutation.ParentStrain,
                Compound = resistanceMutation.Compound,
                ShiftInMIC = resistanceMutation.ShiftInMIC,
                Organization = resistanceMutation.Organization,
                Researcher = resistanceMutation.Researcher,
                Reference = resistanceMutation.Reference,
                Notes = resistanceMutation.Notes,
                DateCreated = DateTime.UtcNow
            });
        }

        public void Apply(GeneResistanceMutationAddedEvent @event)
        {
            _resistanceMutation.Add(@event.ResistanceMutationId, new ResistanceMutation
            {
                ResistanceMutationId = @event.ResistanceMutationId,
                Mutation = @event.Mutation,
                Isolate = @event.Isolate,
                ParentStrain = @event.ParentStrain,
                Compound = @event.Compound,
                ShiftInMIC = @event.ShiftInMIC,
                Organization = @event.Organization,
                Researcher = @event.Researcher,
                Reference = @event.Reference,
                Notes = @event.Notes

            });
        }

        /* Update ResistanceMutation */
        public void UpdateResistanceMutation(ResistanceMutation resistanceMutation)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }

            if (!_resistanceMutation.ContainsKey(resistanceMutation.ResistanceMutationId))
            {
                throw new InvalidOperationException("ResistanceMutation does not exist.");
            }
            if (string.IsNullOrWhiteSpace(resistanceMutation.Mutation))
            {
                throw new InvalidOperationException($" The value of resistanceMutation mutation cannot be null or whitespace");
            }

            RaiseEvent(new GeneResistanceMutationUpdatedEvent
            {
                Id = _id,
                GeneId = _id,
                ResistanceMutationId = resistanceMutation.ResistanceMutationId,
                Mutation = resistanceMutation.Mutation,
                Isolate = resistanceMutation.Isolate,
                ParentStrain = resistanceMutation.ParentStrain,
                Compound = resistanceMutation.Compound,
                ShiftInMIC = resistanceMutation.ShiftInMIC,
                Organization = resistanceMutation.Organization,
                Researcher = resistanceMutation.Researcher,
                Reference = resistanceMutation.Reference,
                Notes = resistanceMutation.Notes,
                DateUpdated = DateTime.UtcNow
            });
        }

        public void Apply(GeneResistanceMutationUpdatedEvent @event)
        {
            _id = @event.Id;
            _resistanceMutation[@event.ResistanceMutationId] = new ResistanceMutation
            {
                ResistanceMutationId = @event.ResistanceMutationId,
                Mutation = @event.Mutation,
                Isolate = @event.Isolate,
                ParentStrain = @event.ParentStrain,
                Compound = @event.Compound,
                ShiftInMIC = @event.ShiftInMIC,
                Organization = @event.Organization,
                Researcher = @event.Researcher,
                Reference = @event.Reference,
                Notes = @event.Notes
            };
        }

        /* Delete ResistanceMutation */
        public void DeleteResistanceMutation(ResistanceMutation resistanceMutation)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (!_resistanceMutation.ContainsKey(resistanceMutation.ResistanceMutationId))
            {
                throw new InvalidOperationException("ResistanceMutation does not exist.");
            }

            RaiseEvent(new GeneResistanceMutationDeletedEvent
            {
                Id = _id,
                GeneId = _id,
                ResistanceMutationId = resistanceMutation.ResistanceMutationId

            });
        }

        public void Apply(GeneResistanceMutationDeletedEvent @event)
        {
            _id = @event.Id;
            _resistanceMutation.Remove(@event.ResistanceMutationId);
        }
    }
}