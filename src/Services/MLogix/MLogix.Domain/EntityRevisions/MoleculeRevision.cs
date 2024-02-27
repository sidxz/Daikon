
using CQRS.Core.Domain.Historical;

namespace MLogix.Domain.EntityRevisions
{
    public class MoleculeRevision : BaseVersionEntity
    {
        public DVariableHistory<string> Name { get; set; }
    }
}