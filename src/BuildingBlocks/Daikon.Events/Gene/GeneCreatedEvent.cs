using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Event;

namespace Daikon.Events.Gene
{
    public class GeneCreatedEvent : BaseEvent
    {
        public GeneCreatedEvent() : base(nameof(GeneCreatedEvent))
        {

        }


        public Guid StrainId { get; set; }
        public string StrainName { get; set; }
        public string AccessionNumber { get; set; }
        public string Name { get; set; }
        public DVariable<string> Function { get; set; }
        public DVariable<string> Product { get; set; }
        public DVariable<string> FunctionalCategory { get; set; }
        
        public DateTime DateCreated { get; set; }

    }
}