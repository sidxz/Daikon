
using CQRS.Core.Domain.Historical;

namespace CQRS.Core.Domain
{
    public abstract class ExpansionPropRevision : BaseVersionEntity
    {
        public DVariableHistory<string> ExpansionValue { get; set; }
    }
}