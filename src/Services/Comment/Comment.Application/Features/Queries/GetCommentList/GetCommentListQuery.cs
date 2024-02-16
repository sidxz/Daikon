using CQRS.Core.Query;
using MediatR;

namespace Comment.Application.Features.Queries.GetCommentList
{
    public class GetCommentListQuery : BaseQuery, IRequest<List<CommentListVM>>
    {
        
    }
}