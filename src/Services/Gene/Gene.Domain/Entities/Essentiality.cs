
using CQRS.Core.Domain;

namespace Gene.Domain.Entities
{
    public class Essentiality : BaseEntity
    {

        public Guid GeneId { get; set; }
        public Guid EssentialityId { get; set; }

        public required DVariable<string> Classification { get; set; }
        public DVariable<string>? Condition { get; set; }
        public DVariable<string>? Method { get; set; }
        public DVariable<string>? Reference { get; set; }
        public DVariable<string>? Note { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Essentiality other)
            {
                return (Classification == null && other.Classification == null || Classification?.Equals(other.Classification) == true) &&
                       (Condition == null && other.Condition == null || Condition?.Equals(other.Condition) == true) &&
                       (Method == null && other.Method == null || Method?.Equals(other.Method) == true) &&
                       (Reference == null && other.Reference == null || Reference?.Equals(other.Reference) == true) &&
                       (Note == null && other.Note == null || Note?.Equals(other.Note) == true);
            }

            return false;
        }

    }
}