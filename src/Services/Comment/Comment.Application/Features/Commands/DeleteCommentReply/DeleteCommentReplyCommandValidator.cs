using FluentValidation;

namespace Comment.Application.Features.Commands.DeleteCommentReply
{
    public class DeleteCommentReplyCommandValidator : AbstractValidator<DeleteCommentReplyCommand>
    {
        public DeleteCommentReplyCommandValidator()
        {
            RuleFor(x => x.ReplyId)
                .NotEmpty()
                .WithMessage("ReplyId is required.");
        }
    }
}