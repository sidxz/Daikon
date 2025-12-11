
using Daikon.Events.Targets;
using CQRS.Core.Comparators;
using Daikon.Shared.Constants.AppTarget;
using Daikon.EventStore.Aggregate;
using Target.Domain.Exceptions;
using Target.Domain.Services;
namespace Target.Domain.Aggregates
{
    public partial class TargetAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        public Dictionary<string, string> _associatedGenes { get; set; }

        public TargetAggregate()
        {

        }


        /* New Target */
        public TargetAggregate(TargetCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _associatedGenes = @event.AssociatedGenes;
            @event.TargetType = @event.AssociatedGenes.Count > 1 ? TargetType.ProteinComplex : TargetType.Protein;
            RaiseEvent(@event);
        }

        public static async Task<TargetAggregate> CreateAsync(TargetCreatedEvent @event, ITargetUniquenessChecker targetUniquenessChecker)
        {
            ArgumentNullException.ThrowIfNull(targetUniquenessChecker);
            ArgumentNullException.ThrowIfNull(@event);

            await EnsureNewTargetAllowed(@event, targetUniquenessChecker).ConfigureAwait(false);

            return new TargetAggregate(@event);
        }

        public static async Task EnsureNewTargetAllowed(TargetCreatedEvent @event, ITargetUniquenessChecker targetUniquenessChecker)
        {
            if (string.IsNullOrWhiteSpace(@event.Name))
            {
                throw new DomainInvariantViolationException("Target name is required.");
            }

            await targetUniquenessChecker.EnsureTargetNameIsUniqueAsync(@event.StrainId, @event.Name).ConfigureAwait(false);
        }

        public void Apply(TargetCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
            _associatedGenes = @event.AssociatedGenes;
        }

        /* Update Target */
        public void UpdateTarget(TargetUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            if (!string.Equals(_Name, @event.Name, StringComparison.Ordinal))
            {
                throw new DomainInvariantViolationException("Target name cannot be modified.");
            }

            if (!_associatedGenes.DictionaryEqual(@event.AssociatedGenes))
            {
                throw new DomainInvariantViolationException("Associated genes cannot be modified using this command.");
            }

            @event.Id = _id;
            @event.Name = _Name;
            @event.AssociatedGenes = _associatedGenes;

            RaiseEvent(@event);
        }

        public void Apply(TargetUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Update Target Associated Genes */
        public void UpdateTargetAssociatedGenes(TargetAssociatedGenesUpdatedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }

            if (_associatedGenes.DictionaryEqual(@event.AssociatedGenes))
            {
                throw new DomainInvariantViolationException("Associated genes are not modified");
            }

            @event.Id = _id;
            @event.Name = _Name;
            @event.TargetType = @event.AssociatedGenes.Count > 1 ? TargetType.ProteinComplex : TargetType.Protein;
            RaiseEvent(@event);
        }

        public void Apply(TargetAssociatedGenesUpdatedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _associatedGenes = @event.AssociatedGenes;
        }

        /* Delete Target */
        public void DeleteTarget(TargetDeletedEvent @event)
        {
            if (!_active)
            {
                throw new DomainInvariantViolationException("This target is already deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(TargetDeletedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _active = false;
        }

        /* Target Rename Event */
        public void RenameTarget(TargetRenamedEvent @event)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This target is deleted.");
            }
            if (_Name == @event.Name)
            {
                throw new InvalidOperationException("Target name is not modified");
            }

            RaiseEvent(@event);
        }

        public void Apply(TargetRenamedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
        }

    }
}