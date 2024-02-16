using CQRS.Core.Domain.Historical;
namespace Comment.Domain.EntityRevisions
{
    public class CommentRevision : BaseVersionEntity
    {
        public DVariableHistory<string> Topic { get; set; }
        public DVariableHistory<string>? Description { get; set; }
       
    }
}