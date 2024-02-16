
using FluentValidation;

namespace Comment.Application.Features.Commands.NewCommentReply
{
    public class NewCommentReplyCommandValidator : AbstractValidator<NewCommentReplyCommand>
    {
        public NewCommentReplyCommandValidator()
        {
        
            RuleFor(t => t.ResourceId)
            .NotEmpty().WithMessage("{ResourceId} is required")
            .NotNull();

        }
        
    }
    
}