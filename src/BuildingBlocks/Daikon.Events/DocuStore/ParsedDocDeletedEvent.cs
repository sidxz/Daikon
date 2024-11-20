using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace Daikon.Events.DocuStore
{
    public class ParsedDocDeletedEvent : BaseEvent
    {
        public ParsedDocDeletedEvent() : base(nameof(ParsedDocDeletedEvent))
        {
            
        }
    }
}