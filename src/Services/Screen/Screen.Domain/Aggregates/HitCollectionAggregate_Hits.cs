using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Screens;
using Screen.Domain.Entities;

namespace Screen.Domain.Aggregates
{
    public partial class HitCollectionAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, Hit> _hits = [];

        /* Add Hit */
        public void AddHit(HitAddedEvent hitAddedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection has been deleted.");
            }

            if (_hits.ContainsKey(hitAddedEvent.HitId))
            {
                throw new Exception("Hit already exists.");
            }
            RaiseEvent(hitAddedEvent);
        }

        public void Apply(HitAddedEvent @event)
        {
            _hits.Add(@event.HitId, new Hit()
            {
                HitCollectionId = @event.Id,
                InitialCompoundStructure = @event.InitialCompoundStructure,
            });
        }

        /* Update Hit */
        public void UpdateHit(HitUpdatedEvent hitUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection has been deleted.");
            }

            if (!_hits.ContainsKey(hitUpdatedEvent.HitId))
            {
                throw new Exception("Hit does not exist.");
            }

            RaiseEvent(hitUpdatedEvent);
        }


        public void Apply(HitUpdatedEvent @event)
        {
            // Get @event.HitId from _hits Dictionary and update it without creating a new HitRecord
            // Only store important parameters necessary for the screen aggregate to run
            // _hits[@event.HitId].InitialCompoundStructure = @event.InitialCompoundStructure;
            // No updates required
        }

        /* Delete Hit */
        public void DeleteHit(HitDeletedEvent hitDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection has been deleted.");
            }

            if (!_hits.ContainsKey(hitDeletedEvent.HitId))
            {
                throw new Exception("Hit does not exist.");
            }
            RaiseEvent(hitDeletedEvent);
        }

        public void Apply(HitDeletedEvent @event)
        {
            _hits.Remove(@event.HitId);
        }
    }
}