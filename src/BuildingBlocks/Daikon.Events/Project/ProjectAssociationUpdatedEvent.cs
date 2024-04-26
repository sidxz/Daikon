using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace Daikon.Events.Project
{
    public class ProjectAssociationUpdatedEvent : BaseEvent
    {
        public ProjectAssociationUpdatedEvent() : base(nameof(ProjectAssociationUpdatedEvent))
        {
            
        }
        public Guid HaId { get; set; }
        public Guid CompoundId { get; set; }
        public string CompoundSMILES { get; set; }
        public Guid HitCompoundId { get; set; }
        public Guid HitId { get; set; }
    }
    
}