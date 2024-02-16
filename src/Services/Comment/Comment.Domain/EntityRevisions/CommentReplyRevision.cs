using CQRS.Core.Domain.Historical;

namespace Comment.Domain.EntityRevisions
{
    public class CommentReplyRevision : BaseVersionEntity
    {
        public DVariableHistory<string>? Body { get; set; }
        
    }
}