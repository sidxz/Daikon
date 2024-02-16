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
        private Guid _hitId;
        private Dictionary<string, string> _associatedHits { get; set; }

        public ProjectAggregate()
        {

        }

        /* New Project */
        public ProjectAggregate(ProjectCreatedEvent ProjectCreatedEvent)
        {
            _active = true;
            _id = ProjectCreatedEvent.Id;
            _Name = ProjectCreatedEvent.Name;
            _compoundId = ProjectCreatedEvent.CompoundId;
            _hitId = ProjectCreatedEvent.HitId;


            RaiseEvent(ProjectCreatedEvent);
        }

        public void Apply(ProjectCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _Name = @event.Name;
            _compoundId = @event.CompoundId;
            _hitId = @event.HitId;

        }

        /* Update Project */
        public void UpdateProject(ProjectUpdatedEvent ProjectUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This Project is deleted.");
            }

            // ProjectUpdatedEvent doesn't allow name or HitId to be changed.
            ProjectUpdatedEvent.Name = _Name;
            ProjectUpdatedEvent.CompoundId = _compoundId;
            ProjectUpdatedEvent.HitId = _hitId;

            RaiseEvent(ProjectUpdatedEvent);
        }

        public void Apply(ProjectUpdatedEvent @event)
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