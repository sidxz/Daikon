
using Daikon.Events.Gene;
using Gene.Domain.Entities;

namespace Gene.Domain.Aggregates
{
    public partial class GeneAggregate
    {

        private readonly Dictionary<Guid, ResistanceMutation> _resistanceMutation = [];

        /* Add ResistanceMutation */
        public void AddResistanceMutation(GeneResistanceMutationAddedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ResistanceMutationId == Guid.Empty)
            {
                throw new InvalidOperationException("ResistanceMutation Id cannot be empty.");
            }
            if (_resistanceMutation.ContainsKey(@event.ResistanceMutationId))
            {
                throw new Exception("ResistanceMutation already exists.");
            }
            if (string.IsNullOrWhiteSpace(@event.Mutation))
            {
                throw new InvalidOperationException($"The value of resistanceMutation mutation cannot be null or whitespace");
            }
            RaiseEvent(@event);
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
        public void UpdateResistanceMutation(GeneResistanceMutationUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.ResistanceMutationId == Guid.Empty)
            {
                throw new InvalidOperationException("ResistanceMutation Id cannot be empty.");
            }
            if (!_resistanceMutation.ContainsKey(@event.ResistanceMutationId))
            {
                throw new InvalidOperationException("ResistanceMutation does not exist.");
            }
            if (string.IsNullOrWhiteSpace(@event.Mutation))
            {
                throw new InvalidOperationException($"The value of resistanceMutation mutation cannot be null or whitespace");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneResistanceMutationUpdatedEvent @event)
        {
            _resistanceMutation[@event.ResistanceMutationId].Mutation = @event.Mutation;
            _resistanceMutation[@event.ResistanceMutationId].Isolate = @event.Isolate;
            _resistanceMutation[@event.ResistanceMutationId].ParentStrain = @event.ParentStrain;
            _resistanceMutation[@event.ResistanceMutationId].Compound = @event.Compound;
            _resistanceMutation[@event.ResistanceMutationId].ShiftInMIC = @event.ShiftInMIC;
            _resistanceMutation[@event.ResistanceMutationId].Organization = @event.Organization;
            _resistanceMutation[@event.ResistanceMutationId].Researcher = @event.Researcher;
            _resistanceMutation[@event.ResistanceMutationId].Reference = @event.Reference;
            _resistanceMutation[@event.ResistanceMutationId].Notes = @event.Notes;
        }

        /* Delete ResistanceMutation */
        public void DeleteResistanceMutation(GeneResistanceMutationDeletedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This gene is deleted.");
            }
            if (@event.ResistanceMutationId == Guid.Empty)
            {
                throw new InvalidOperationException("ResistanceMutation Id cannot be empty.");
            }
            if (!_resistanceMutation.ContainsKey(@event.ResistanceMutationId))
            {
                throw new InvalidOperationException("ResistanceMutation does not exist.");
            }

            RaiseEvent(@event);
        }

        public void Apply(GeneResistanceMutationDeletedEvent @event)
        {
            _resistanceMutation.Remove(@event.ResistanceMutationId);
        }
    }
}