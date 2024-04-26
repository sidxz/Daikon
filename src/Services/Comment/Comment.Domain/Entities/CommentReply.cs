using CQRS.Core.Domain;

namespace Comment.Domain.Entities
{
    public class CommentReply : BaseEntity
    {
        public Guid CommentId { get; set; }
        public Guid ReplyId { get; set; }
        public DVariable<string> Body { get; set; }
        public HashSet<string> Tags { get; set; }
        public HashSet<Guid> Mentions { get; set; }
        public HashSet<Guid> Subscribers { get; set; }
        
    }
}