using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace CQRS.Core.Domain
{
    /// <summary>
    /// Base class for all aggregate roots in the domain.
    /// </summary>
    public abstract class AggregateRoot
    {
        protected Guid _id;
        public Guid Id => _id;
        private readonly List<BaseEvent> _changes = new();
        public int Version { get; set; } = -1;

        /// <summary>
        /// Gets all uncommitted changes made to the aggregate root.
        /// </summary>
        public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes.AsEnumerable();

        /// <summary>
        /// Marks all changes made to the aggregate root as committed.
        /// </summary>
        public void MarkChangesAsCommitted() => _changes.Clear();

        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            var method = this.GetType()
                .GetMethod("Apply", [@event.GetType()]) 
                ?? throw new 
                NotImplementedException($"Apply method not implemented for {@event.GetType().Name}");

            method.Invoke(this, [@event]);

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        /// <summary>
        /// Raises an event and applies it to the aggregate root.
        /// </summary>
        /// <param name="event">The event to raise and apply.</param>
        protected void RaiseEvent(BaseEvent @event) => ApplyChange(@event, true);

        /// <summary>
        /// Replays a sequence of events on the aggregate root.
        /// </summary>
        /// <param name="events">The events to replay.</param>
        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyChange(@event, false);
                Version++;
            }
        }
    }
}