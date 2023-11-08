using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Messages;

namespace CQRS.Core.Event
{
    /// <summary>
    /// Base class for all events in the CQRS pattern.
    /// </summary>
    public abstract class BaseEvent : Message
    {
        protected BaseEvent(string type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the version of the event.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        public string Type { get; set; }

    }
}