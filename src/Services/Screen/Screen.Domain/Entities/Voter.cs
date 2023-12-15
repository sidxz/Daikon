

using CQRS.Core.Domain;

namespace Screen.Domain.Entities
{
    public class Voter : BaseEntity
    {
        public Guid HitId { get; set; }
        public required Guid VoterId { get; set; }
        public DVariable<bool> VotedPositive { get; set; }
        public DVariable<bool> VotedNeutral { get; set; }
        public DVariable<bool> VotedNegative { get; set; }
        public DVariable<DateTime> VotedOn { get; set; }
        public DVariable<DateTime> UpdatedOn { get; set; }
        public DVariable<string>? Comment { get; set; }
    }
}