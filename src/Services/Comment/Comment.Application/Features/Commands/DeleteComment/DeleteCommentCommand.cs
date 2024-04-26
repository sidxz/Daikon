using CQRS.Core.Command;
using MediatR;

namespace Comment.Application.Features.Commands.DeleteComment
{
    public class DeleteCommentCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}