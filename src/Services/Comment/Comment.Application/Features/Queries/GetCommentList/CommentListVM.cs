

namespace Comment.Application.Features.Queries.GetCommentList
{
    public class CommentListVM
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public object Topic { get; set; }
        public object Description { get; set; }
        public HashSet<string> Tags { get; set; }
        public HashSet<Guid> Mentions { get; set; }
        public HashSet<Guid> Subscribers { get; set; }
        public bool IsCommentLocked { get; set; }
    }
}