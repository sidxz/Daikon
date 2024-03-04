
using CQRS.Core.Domain;

namespace CQRS.Core.Event
{

    public abstract class BaseEvent :  DocMetadata
    {
        protected BaseEvent(string type)
        {
            this.Type = type;
        }

        public Guid Id { get; set; }
        public Guid RequestorUserId { get; set;}
        public int Version { get; set; }
        public string Type { get; set; }

        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

    }
}