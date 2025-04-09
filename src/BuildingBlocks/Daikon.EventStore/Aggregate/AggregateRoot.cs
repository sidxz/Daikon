/*
== Overview ==
The AggregateRoot class, part of the CQRS.Core.Domain namespace, is an abstract base class designed to represent 
the root of an aggregate in a system using the CQRS (Command Query Responsibility Segregation) and Event Sourcing patterns. 
It provides a foundation for implementing domain entities that encapsulate business logic and maintain state through events

== Design Considerations ==
The class leverages event sourcing principles, ensuring that all state changes are captured as a sequence of events.
Reflection is used to dynamically find and invoke Apply methods, providing flexibility but also requiring careful implementation in derived classes.
The use of a version number supports concurrency control and conflict resolution in distributed systems.

== Key Properties and Fields ==
_id (Guid): A protected field that holds the unique identifier of the aggregate.
Id (Guid): A public read-only property that exposes the aggregate's identifier.
_changes (List<BaseEvent>): A private list that tracks changes (events) that have not yet been committed to the event store.
Version (int): Public property representing the version of the aggregate, initialized to -1.

== Usage ==
- As an abstract class, AggregateRoot is intended to be inherited by concrete aggregate classes in a domain model.
- It provides mechanisms for tracking and applying events, which are central to maintaining the state of the aggregate in an event-sourced system.
- Derived classes should implement specific Apply methods for different event types to define how each event affects the state of the aggregate.

*/

using Daikon.EventStore.Event;

namespace Daikon.EventStore.Aggregate
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        public Guid Id => _id;
        private readonly List<BaseEvent> _changes = new();
        public int Version { get; set; } = -1;



        /*  
         GetUncommittedChanges()
            Returns an IEnumerable<BaseEvent> representing all changes (events) that have not yet been committed to the event store.
        */
        public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes.AsEnumerable();



        /* 
         MarkChangesAsCommitted()
            Clears the list of uncommitted changes, indicating that these changes have been persisted.
        */
        public void MarkChangesAsCommitted() => _changes.Clear();


        /* 
         ApplyChange(BaseEvent, bool)

            A private method that applies a given event to the aggregate.
            It uses reflection to find and invoke the appropriate Apply method for the event type.
            If the event is new (indicated by the isNew parameter), it adds the event to the list of uncommitted changes.
            Throws NotImplementedException if the corresponding Apply method is not implemented.
        */
        private static readonly Dictionary<Type, Action<AggregateRoot, BaseEvent>> _applyMethodsCache = new();

        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            var eventType = @event.GetType();

            if (!_applyMethodsCache.TryGetValue(eventType, out var applyAction))
            {
                var methodInfo = this.GetType().GetMethod("Apply", new[] { eventType });
                if (methodInfo == null)
                    throw new NotImplementedException($"Apply method not implemented for {eventType.Name}");

                // Create delegate (Action<AggregateRoot, BaseEvent>) using reflection
                applyAction = (aggregate, evt) => methodInfo.Invoke(aggregate, new[] { evt });
                _applyMethodsCache[eventType] = applyAction;
            }

            applyAction(this, @event);

            if (isNew)
            {
                _changes.Add(@event);
            }
        }


        /*
         RaiseEvent(BaseEvent): 
            A protected method used to raise a new event. It applies the event and marks it as new.
        */
        protected void RaiseEvent(BaseEvent @event) => ApplyChange(@event, true);



        /*
         ReplayEvents(IEnumerable<BaseEvent>):

            Applies a sequence of events to the aggregate, typically used to rebuild the aggregate's state from its event history.
            Increments the Version of the aggregate for each applied event.
        */
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