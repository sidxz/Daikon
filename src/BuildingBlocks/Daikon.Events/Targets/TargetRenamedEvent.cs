using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.EventStore.Event;

namespace Daikon.Events.Targets
{
    public class TargetRenamedEvent : BaseEvent
    {
        public TargetRenamedEvent() : base(nameof(TargetRenamedEvent))
        {

        }
        public required string Name { get; set; }

    }
}