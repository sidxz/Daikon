using CQRS.Core.Domain;
namespace Comment.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid ResourceId { get; set; }
        public DVariable<string> Topic { get; set; }
        public DVariable<string> Description { get; set; }
        public HashSet<string> Tags { get; set; }
        public HashSet<Guid> Mentions { get; set; }
        public HashSet<Guid> Subscribers { get; set; }
        public bool IsCommentLocked { get; set; }

    }
}