
using CQRS.Core.Domain;
using Daikon.Events.MLogix;

namespace MLogix.Domain.Aggregates
{
    public partial class MoleculeAggregate : AggregateRoot
    {
        private bool _active;
        private Guid _registrationId;

        public MoleculeAggregate()
        {

        }

        /* New Molecule */
        public MoleculeAggregate(MoleculeCreatedEvent @event)
        {
            _id = @event.Id;
            _registrationId = @event.RegistrationId;
            _active = true;

            RaiseEvent(@event);
        }

        public void Apply(MoleculeCreatedEvent @event)
        {
            _id = @event.Id;
            _registrationId = @event.RegistrationId;
            _active = true;
        }

        // Update Molecule
        public void UpdateMolecule(MoleculeUpdatedEvent @event)
        {
            if (!_active)
                throw new InvalidOperationException("Molecule is not active");

            // Allow changes to registration id
            _registrationId = @event.RegistrationId;

            RaiseEvent(@event);
        }

        public void Apply(MoleculeUpdatedEvent @event)
        {
            _registrationId = @event.RegistrationId;
        }

        public void DiscloseMolecule(MoleculeDisclosedEvent @event)
        {
            if (!_active)
                throw new InvalidOperationException("Molecule is not active");

            RaiseEvent(@event);
        }

        public void Apply(MoleculeDisclosedEvent @event)
        {
            // Do nothing
        }

        // Delete Molecule
        public void DeleteMolecule(MoleculeDeletedEvent @event)
        {
            if (!_active)
                throw new InvalidOperationException("Molecule is not active");

            _active = false;

            RaiseEvent(@event);
        }

        public void Apply(MoleculeDeletedEvent @event)
        {
            _active = false;
        }
    }
}