using Daikon.EventStore.Event;

namespace Daikon.EventStore.Producers
{
    /*
     Defines a contract for producing domain events to an external message broker (e.g., Kafka).
    */
    public interface IEventProducer
    {
        /*
         Produces the given event to the specified topic.
         
         Parameters:
         - topic: The destination topic/queue/channel.
         - @event: The event instance to be serialized and sent.

         Throws:
         - Exception if delivery fails after retries or due to fatal errors.
        */
        Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent;
    }
}
