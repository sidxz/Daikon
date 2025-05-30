using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.EventStore.Event;

namespace Daikon.Events.DocuStore
{
    public class ParsedDocDeletedEvent : BaseEvent
    {
        public ParsedDocDeletedEvent() : base(nameof(ParsedDocDeletedEvent))
        {
            
        }
    }
}