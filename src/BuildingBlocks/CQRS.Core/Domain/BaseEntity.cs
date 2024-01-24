

namespace CQRS.Core.Domain
{
    public abstract class BaseEntity : DocMetadata
    {
        public Guid Id { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDeleted { get; set; }
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

    }
}