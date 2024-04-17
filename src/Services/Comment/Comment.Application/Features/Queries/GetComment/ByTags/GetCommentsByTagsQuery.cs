using CQRS.Core.Query;
using MediatR;

namespace Comment.Application.Features.Queries.GetComment.ByTags
{
    public class GetCommentsByTagsQuery : BaseQuery, IRequest<List<CommentVM>>
    {
        public HashSet<string> Tags { get; set; }
        
    }
}