using FluentValidation;

namespace Comment.Application.Features.Commands.UpdateCommentReply
{
    public class UpdateCommentReplyCommandValidator : AbstractValidator<UpdateCommentReplyCommand>
    {
        public UpdateCommentReplyCommandValidator()
        {
            
            RuleFor(t => t.ResourceId)
            .NotEmpty().WithMessage("{ResourceId} is required");

        }
    }
    
}