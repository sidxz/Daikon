
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Screens;

namespace Screen.Domain.Aggregates
{
    public partial class ScreenAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private IMapper _mapper;
        public ScreenAggregate()
        {
            
        }

        /* New Screen */
        public ScreenAggregate(Entities.Screen screen, IMapper mapper)
        {
            _active = true;
            _id = screen.Id;
            _Name = screen.Name;
            _mapper = mapper;

            var screenCreatedEvent = mapper.Map<ScreenCreatedEvent>(screen);

            RaiseEvent(screenCreatedEvent);
        }

        public void Apply(ScreenCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
        }

        /* Update Screen */
        public void UpdateScreen(Entities.Screen screen)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            var screenUpdatedEvent = _mapper.Map<ScreenUpdatedEvent>(screen);
            screenUpdatedEvent.Id = screen.Id;
            screenUpdatedEvent.Name = screen.Name;

            RaiseEvent(screenUpdatedEvent);
        }

        public void Apply(ScreenUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        /* Update Screen Associated Genes */
        public void UpdateScreenAssociatedGenes(Dictionary<string, string> associatedTargets)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            var screenAssociatedTargetsUpdatedEvent = new ScreenAssociatedTargetsUpdatedEvent()
            {
                Id = _id,
                Name = _Name,
                AssociatedTargets = associatedTargets
            };
            RaiseEvent(screenAssociatedTargetsUpdatedEvent);
        }

        /* Delete Screen */
        public void DeleteScreen(Entities.Screen screen)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is already deleted.");
            }

            var screenDeletedEvent = new ScreenDeletedEvent
            {
                Id = screen.Id,
                Name = _Name
            };

            RaiseEvent(screenDeletedEvent);
        }

        public void Apply(ScreenDeletedEvent @event)
        {
            _id = @event.Id;
            _Name = @event.Name;
            _active = false;
        }
    }
}