// Purpose: Domain model for ExpansionProps.
namespace CQRS.Core.Domain
{
    public abstract class ExpansionProp : BaseEntity
    {
        public required string ExpansionType { get; set; }
        public DVariable<string> ExpansionValue { get; set; }

    }
}