
using Daikon.Events.Project;
using Daikon.EventStore.Aggregate;

namespace Project.Domain.Aggregates
{
    public partial class ProjectAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Guid _compoundId;

        public ProjectAggregate()
        {

        }

        /* New Project */
        public ProjectAggregate(ProjectCreatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (@event.Name == null)
            {
                throw new InvalidOperationException("Name cannot be empty.");
            }

            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _compoundId = @event.CompoundId;
            RaiseEvent(@event);
        }

        public void Apply(ProjectCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _compoundId = @event.CompoundId;
        }

        /* Update Project */
        public void UpdateProject(ProjectUpdatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }

            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(ProjectUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Rename Project */
        public void RenameProject(ProjectRenamedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }

            if (@event.Name == null)
            {
                throw new InvalidOperationException("Name cannot be empty.");
            }

            if (@event.Name == _Name)
            {
                throw new InvalidOperationException("Name is the same as the current name.");
            }

            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            RaiseEvent(@event);
        }
        public void Apply(ProjectRenamedEvent @event)
        {
            _Name = @event.Name;
        }

        /* Update HA Association */
        public void UpdateHaAssociation(ProjectAssociationUpdatedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }

            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            RaiseEvent(@event);
        }

        public void Apply(ProjectAssociationUpdatedEvent @event)
        {
            _id = @event.Id;
            _compoundId = @event.CompoundId;
        }

        /* Delete Project */
        public void DeleteProject(ProjectDeletedEvent @event)
        {
            if (@event.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Event Id cannot be empty.");
            }
            if (!_active)
            {
                throw new InvalidOperationException("This Project is already deleted.");
            }

            RaiseEvent(@event);
        }
        public void Apply(ProjectDeletedEvent @event)
        {
            _active = false;
        }
    }
}