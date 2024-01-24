

using CQRS.Core.Domain;
using CQRS.Core.Comparators;
using Daikon.Events.Screens;

namespace Screen.Domain.Aggregates
{
    public partial class ScreenAggregate : AggregateRoot
    {
        private bool _active;
        private string _name;
        public Dictionary<string, string> _associatedTargets { get; set; }
        public ScreenAggregate()
        {

        }

        /* New Screen */
        public ScreenAggregate(ScreenCreatedEvent screenCreatedEvent)
        {
            _active = true;
            _id = screenCreatedEvent.Id;
            _name = screenCreatedEvent.Name;
            _associatedTargets = screenCreatedEvent.AssociatedTargets;

            RaiseEvent(screenCreatedEvent);
        }

        public void Apply(ScreenCreatedEvent @event)
        {
            _active = true;
            _id = @event.Id;
            _name = @event.Name;
            _associatedTargets = @event.AssociatedTargets;
        }



        /* Update Screen */
        public void UpdateScreen(ScreenUpdatedEvent screenUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            // ScreenUpdatedEvent doesn't allow name or associated targets to be changed.
            screenUpdatedEvent.Name = _name;
            screenUpdatedEvent.AssociatedTargets = _associatedTargets;

            RaiseEvent(screenUpdatedEvent);
        }

        public void Apply(ScreenUpdatedEvent @event)
        {
            _id = @event.Id;
        }



        /* Delete Screen */
        public void DeleteScreen(ScreenDeletedEvent screenDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is already deleted.");
            }

            RaiseEvent(screenDeletedEvent);
        }

        public void Apply(ScreenDeletedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }




        /* Update Screen Associated Targets */
        public void UpdateScreenAssociatedTargets(ScreenAssociatedTargetsUpdatedEvent screenAssociatedTargetsUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            if (_associatedTargets.DictionaryEqual(screenAssociatedTargetsUpdatedEvent.AssociatedTargets))
            {
                throw new InvalidOperationException("Associated targets are not modified");
            }

            screenAssociatedTargetsUpdatedEvent.Name = _name;
            RaiseEvent(screenAssociatedTargetsUpdatedEvent);
        }

        public void Apply(ScreenAssociatedTargetsUpdatedEvent @event)
        {
            _id = @event.Id;
            _associatedTargets = @event.AssociatedTargets;
        }




        /* Screen Rename Event */
        public void RenameScreen(ScreenRenamedEvent screenRenamedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }
            if (_name == screenRenamedEvent.Name)
            {
                throw new InvalidOperationException("Screen name is not modified");
            }

            RaiseEvent(screenRenamedEvent);
        }

        public void Apply(ScreenRenamedEvent @event)
        {
            _id = @event.Id;
            _name = @event.Name;
        }
    }
}