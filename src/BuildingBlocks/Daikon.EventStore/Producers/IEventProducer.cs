
using Daikon.EventStore.Event;

namespace Daikon.EventStore.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent;
    }
}