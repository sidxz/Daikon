
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Screens;
using Screen.Domain.Entities;

namespace Screen.Domain.Aggregates
{
    public partial class HitCollectionAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Guid _ScreenId;

        private IMapper _mapper;


        public HitCollectionAggregate()
        {
        }

        /* Add HitCollection */
        public  HitCollectionAggregate(HitCollection hitCollection, IMapper mapper)
        {
            _active = true;
            _id = hitCollection.Id;
            _Name = hitCollection.Name;
            _ScreenId = hitCollection.ScreenId;
            _mapper = mapper;

            var hitCollectionCreatedEvent = mapper.Map<HitCollectionCreatedEvent>(hitCollection);

            RaiseEvent(hitCollectionCreatedEvent);
        }

        public void Apply(HitCollectionCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
            _ScreenId = @event.ScreenId;
        }

        /* Update HitCollection */
        public void UpdateHitCollection(HitCollection hitCollection)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is deleted.");
            }

            var hitCollectionUpdatedEvent = _mapper.Map<HitCollectionUpdatedEvent>(hitCollection);
            hitCollectionUpdatedEvent.Id = _id;
            hitCollectionUpdatedEvent.HitCollectionId = _id;
            hitCollectionUpdatedEvent.ScreenId = _ScreenId;
            hitCollectionUpdatedEvent.Name = hitCollection.Name;

            RaiseEvent(hitCollectionUpdatedEvent);
        }

        public void Apply(HitCollectionUpdatedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
        }

        /* Delete HitCollection */
        public void DeleteHitCollection(Guid id)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This hitCollection is already deleted.");
            }

            var hitCollectionDeletedEvent = new HitCollectionDeletedEvent()
            {
                Id = _id,
                HitCollectionId = _id,
                Name = _Name,
                ScreenId = _ScreenId
            };
            hitCollectionDeletedEvent.Id = id;

            RaiseEvent(hitCollectionDeletedEvent);
        }

        public void Apply(HitCollectionDeletedEvent @event)
        {
            _active = false;
        }
    }
}