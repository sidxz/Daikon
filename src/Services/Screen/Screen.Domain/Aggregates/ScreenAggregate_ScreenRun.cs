
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Screens;
using Screen.Domain.Entities;

namespace Screen.Domain.Aggregates
{
    public partial class ScreenAggregate : AggregateRoot
    {
        private readonly Dictionary<Guid, ScreenRun> _screenRuns = [];


        /* Add Screen Run */
        public void AddScreenRun(ScreenRun screenRun, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            if (_screenRuns.ContainsKey(screenRun.ScreenRunId))
            {
                throw new Exception("Screen Run already exists");
            }
            _mapper = mapper;
            var screenRunAddedEvent = _mapper.Map<ScreenRunAddedEvent>(screenRun);
            screenRunAddedEvent.Id = _id;
            screenRunAddedEvent.ScreenId = _id;
            screenRunAddedEvent.ScreenRunId = screenRun.ScreenRunId;
            RaiseEvent(screenRunAddedEvent);
        }


        public void Apply(ScreenRunAddedEvent @event)
        {
            _screenRuns.Add(@event.ScreenRunId, _mapper.Map<ScreenRun>(@event));
        }

        /* Update Screen Run */
        public void UpdateScreenRun(ScreenRun screenRun, IMapper mapper)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            if (!_screenRuns.ContainsKey(screenRun.ScreenRunId))
            {
                throw new Exception("Screen Run does not exist");
            }
            _mapper = mapper;

            var screenRunUpdatedEvent = _mapper.Map<ScreenRunUpdatedEvent>(screenRun);
            screenRunUpdatedEvent.Id = _id;
            screenRunUpdatedEvent.ScreenId = _id;
            screenRunUpdatedEvent.ScreenRunId = screenRun.ScreenRunId;
            RaiseEvent(screenRunUpdatedEvent);
        }

        public void Apply(ScreenRunUpdatedEvent @event)
        {
            _screenRuns[@event.ScreenRunId] = _mapper.Map<ScreenRun>(@event);
        }

        /* Delete Screen Run */
        public void DeleteScreenRun(Guid screenRunId)
        {
            if (!_active)
            {
                throw new InvalidOperationException("This screen is deleted.");
            }

            if (!_screenRuns.ContainsKey(screenRunId))
            {
                throw new Exception("Screen Run does not exist");
            }

            var screenRunDeletedEvent = new ScreenRunDeletedEvent()
            {
                Id = _id,
                ScreenId = _id,
                ScreenRunId = screenRunId
            };
            RaiseEvent(screenRunDeletedEvent);
        }

        public void Apply(ScreenRunDeletedEvent @event)
        {
            _screenRuns.Remove(@event.ScreenRunId);
        }


    }
}