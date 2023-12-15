
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class ProteinProductionRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> Production { get; set; }
        public DVariableHistory<string>? Method { get; set; }
        public DVariableHistory<string>? Purity { get; set; }
        public DVariableHistory<DateTime>? DateProduced { get; set; }
        public DVariableHistory<string>? PMID { get; set; }
        public DVariableHistory<string>? Notes { get; set; }
        public DVariableHistory<string>? URL { get; set; }
    }
}