using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace CQRS.Core.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync<TEvent>(string topic, TEvent @event) where TEvent : BaseEvent;
    }
}