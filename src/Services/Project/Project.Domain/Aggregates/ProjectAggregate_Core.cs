using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.Events.Project;

namespace Project.Domain.Aggregates
{
    public partial class ProjectAggregate: AggregateRoot
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
        public void UpdateProject(ProjectUpdatedEvent ProjectUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            RaiseEvent(ProjectUpdatedEvent);
        }

        public void Apply(ProjectUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Update HA Association */
        public void UpdateHaAssociation(ProjectAssociationUpdatedEvent ProjectAssociationUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            RaiseEvent(ProjectAssociationUpdatedEvent);
        }

        public void Apply(ProjectAssociationUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Delete Project */
        public void DeleteProject(ProjectDeletedEvent ProjectDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is already deleted.");
            }

            RaiseEvent(ProjectDeletedEvent);
        }
        public void Apply(ProjectDeletedEvent @event)
        {
            _active = false;
        }
    }
}