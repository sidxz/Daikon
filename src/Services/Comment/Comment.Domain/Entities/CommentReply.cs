using CQRS.Core.Domain;

namespace Comment.Domain.Entities
{
    public class CommentReply : BaseEntity
    {
        public Guid CommentId { get; set; }
        public Guid ResourceId { get; set; }
        public DVariable<string>? Body { get; set; }
        public string? PostedBy { get; set; }
        
    }
}