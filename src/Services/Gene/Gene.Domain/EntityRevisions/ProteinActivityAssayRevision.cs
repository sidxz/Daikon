
using CQRS.Core.Domain.Historical;

namespace Gene.Domain.EntityRevisions
{
    public class ProteinActivityAssayRevision : BaseVersionEntity
    {
        public Guid GeneId { get; set; }
        public DVariableHistory<string> Assay { get; set; }
        public DVariableHistory<string>? Method { get; set; }
        public DVariableHistory<string>? Throughput { get; set; }
        public DVariableHistory<string>? PMID { get; set; }
        public DVariableHistory<string>? Reference { get; set; }
        public DVariableHistory<string>? URL { get; set; }
    }
}