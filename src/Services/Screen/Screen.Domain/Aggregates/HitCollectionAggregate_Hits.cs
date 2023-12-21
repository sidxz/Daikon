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
        public void AddHit(Hit hit, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is deleted.");
            }

            if (_hits.ContainsKey(hit.HitId))
            {
                throw new Exception("Hit already exists");
            }
            _mapper = mapper;
            var hitAddedEvent = mapper.Map<HitAddedEvent>(hit);
            hitAddedEvent.Id = _id;
            hitAddedEvent.HitCollectionId = _id;
            hitAddedEvent.HitId = hit.HitId;
            RaiseEvent(hitAddedEvent);
        }

        public void Apply(HitAddedEvent @event)
        {
            _hits.Add(@event.HitId, _mapper.Map<Hit>(@event));
        }

        /* Update Hit */
        public void UpdateHit(Hit hit, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is deleted.");
            }

            if (!_hits.ContainsKey(hit.HitId))
            {
                throw new Exception("Hit does not exist");
            }
            _mapper = mapper;

            var hitUpdatedEvent = _mapper.Map<HitUpdatedEvent>(hit);
            hitUpdatedEvent.Id = _id;
            hitUpdatedEvent.HitCollectionId = _id;
            hitUpdatedEvent.HitId = hit.HitId;
            RaiseEvent(hitUpdatedEvent);
        }

        public void Apply(HitUpdatedEvent @event)
        {
            _hits[@event.HitId] = _mapper.Map<Hit>(@event);
        }

        /* Delete Hit */
        public void DeleteHit(Guid hitId)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is deleted.");
            }

            if (!_hits.ContainsKey(hitId))
            {
                throw new Exception("Hit does not exist");
            }

            var hitDeletedEvent = new HitDeletedEvent
            {
                Id = _id,
                HitCollectionId = _id,
                HitId = hitId
            };
            RaiseEvent(hitDeletedEvent);
        }

        public void Apply(HitDeletedEvent @event)
        {
            _hits.Remove(@event.HitId);
        }        
    }
}