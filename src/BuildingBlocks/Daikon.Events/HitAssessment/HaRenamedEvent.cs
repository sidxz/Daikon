using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace Daikon.Events.HitAssessment
{
    public class HaRenamedEvent : BaseEvent
    {
        public HaRenamedEvent() : base(nameof(HaRenamedEvent))
        {

        }
        public required string Name { get; set; }

    }

}