using CQRS.Core.Query;
using MediatR;

namespace Comment.Application.Features.Queries.GetComment.ById
{
    public class GetCommentByIdQuery : BaseQuery, IRequest<CommentVM>
    {
        public Guid Id { get; set; }
        
    }
}