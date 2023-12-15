
using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class HitCollection : BaseEntity
    {
        public Guid ScreenId { get; set; }
        public string? Name { get; set; }
        public required string Type { get; set; }
        public DVariable<string>? Notes { get; set; }
        public DVariable<string>? Owner { get; set; }
    }
}