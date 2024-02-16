using CQRS.Core.Command;
using MediatR;

namespace Comment.Application.Features.Commands.DeleteCommentReply
{
    public class DeleteCommentReplyCommand : BaseCommand, IRequest<Unit>
    {
        public Guid ReplyId { get; set; }
        
    }
}