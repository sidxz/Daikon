using Comment.Application.Features.Queries.GetComment;
using CQRS.Core.Query;
using MediatR;

namespace Comment.Application.Features.Queries.GetMostRecent
{
    public class GetMostRecentQuery : BaseQuery, IRequest<List<CommentVM>>
    {
        public int Count { get; set; }
    }
}