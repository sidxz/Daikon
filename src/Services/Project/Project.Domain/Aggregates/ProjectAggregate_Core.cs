using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.Events.Project;

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