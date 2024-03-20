
using CQRS.Core.Domain;

namespace Target.Domain.Entities
{
    public class PQResponse : BaseEntity
    {
        // The target id is set when the PQResponse is approved, along with TargetCreatedEvent
        public Guid? TargetId { get; set; }
        public Guid RequestedBy { get; set; }
        public required string RequestedTargetName { get; set; }
        public required string ApprovedTargetName { get; set; }
        public Dictionary<string, string>? RequestedAssociatedGenes { get; set; }
        public Dictionary<string, string>? ApprovedAssociatedGenes { get; set; }
        public Guid StrainId { get; set; }
        public List<Tuple<string, string, string>> Response { get; set; }

    }
}