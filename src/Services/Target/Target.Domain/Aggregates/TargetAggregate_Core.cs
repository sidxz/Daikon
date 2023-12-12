
using AutoMapper;
using CQRS.Core.Domain;
using Daikon.Events.Targets;

namespace Target.Domain.Aggregates
{
    public  class TargetAggregate : AggregateRoot
    {
        private bool _active;
        private string _Name;
        private Entities.Target target;

        public TargetAggregate()
        {

        }


        /* New Target */
        public TargetAggregate(Entities.Target target, IMapper mapper)
        {
            _active = true;
            _id = target.Id;
            _Name = target.Name;

            var targetCreatedEvent = mapper.Map<TargetCreatedEvent>(target);

            RaiseEvent(targetCreatedEvent);
        }

        public void Apply(TargetCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _Name = @event.Name;
        }
    }
}