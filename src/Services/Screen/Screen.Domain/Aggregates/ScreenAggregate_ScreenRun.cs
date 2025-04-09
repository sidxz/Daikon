using Daikon.Events.Screens;
using Daikon.EventStore.Aggregate;
using Screen.Domain.Entities;

namespace Screen.Domain.Aggregates
{
    public partial class ScreenAggregate : AggregateRoot
    {
        private  Dictionary<Guid, ScreenRun> _screenRuns = [];


        /* Add Screen Run */
        public void AddScreenRun(ScreenRunAddedEvent screenRunAddedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen has been deleted.");
            }

            if (_screenRuns.ContainsKey(screenRunAddedEvent.ScreenRunId))
            {
                throw new Exception("Screen Run already exists.");
            }
            RaiseEvent(screenRunAddedEvent);
        }


        public void Apply(ScreenRunAddedEvent @event)
        {
            // Store important parameters necessary for the screen aggregate to run
            _screenRuns.Add(@event.ScreenRunId, new ScreenRun()
            {
                ScreenId = @event.Id,
                Library = @event.Library,
            });
        }

        /* Update Screen Run */
        public void UpdateScreenRun(ScreenRunUpdatedEvent screenRunUpdatedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen has been deleted.");
            }

            if (!_screenRuns.ContainsKey(screenRunUpdatedEvent.ScreenRunId))
            {
                throw new Exception("Screen Run does not exist.");
            }

            RaiseEvent(screenRunUpdatedEvent);
        }

        public void Apply(ScreenRunUpdatedEvent @event)
        {

            // Update the existing ScreenRun identified by @event.ScreenRunId without creating a new one
            // Store important parameters necessary for the screen aggregate to run
            _screenRuns[@event.ScreenRunId].Library = @event.Library;

        }

        /* Delete Screen Run */
        public void DeleteScreenRun(ScreenRunDeletedEvent screenRunDeletedEvent)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen has been deleted.");
            }

            if (!_screenRuns.ContainsKey(screenRunDeletedEvent.ScreenRunId))
            {
                throw new Exception("Screen Run does not exist.");
            }

            RaiseEvent(screenRunDeletedEvent);
        }

        public void Apply(ScreenRunDeletedEvent @event)
        {
            _screenRuns.Remove(@event.ScreenRunId);
        }


    }
}