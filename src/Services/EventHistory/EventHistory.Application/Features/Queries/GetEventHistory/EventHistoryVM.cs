using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Daikon.EventStore.Event;

namespace EventHistory.Application.Features.Queries.GetEventHistory
{
    public class EventHistoryVM : VMMeta
    {
        public string Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid AggregateIdentifier { get; set; }
        public string AggregateType { get; set; }
        public string Link { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public string EventMessage { get; set; }
        public string? UserId { get; set; }

    }
}