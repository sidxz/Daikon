using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneDeletedEvent : BaseEvent
    {
        public GeneDeletedEvent() : base(nameof(GeneDeletedEvent))
        {

        }
        public string AccessionNumber { get; set; }
    }
}