
using CQRS.Core.Domain.Historical;

namespace Screen.Domain.EntityRevisions
{
    public class HitCollectionRevision : BaseVersionEntity
    {
        public DVariableHistory<string>? Notes { get; set; }
        public DVariableHistory<string>? Owner { get; set; }
    }
}