
using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class HitCollection : BaseEntity
    {
        public Guid HitCollectionId { get; set; }
        public Guid ScreenId { get; set; }
        public required string Name { get; set; }
        public required string HitCollectionType { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? Owner { get; set; }
    }
}