using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Event;

namespace Daikon.Events.Project
{
    public class ProjectRenamedEvent : BaseEvent
    {
        public ProjectRenamedEvent() : base(nameof(ProjectRenamedEvent))
        {

        }
        public required string Name { get; set; }

    }

}