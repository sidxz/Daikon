
using FluentValidation;

namespace Comment.Application.Features.Commands.NewComment
{
    public class NewCommentCommandValidator : AbstractValidator<NewCommentCommand>
    {
        public NewCommentCommandValidator()
        {
            
            RuleFor(t => t.ResourceId)
            .NotEmpty().WithMessage("{ResourceId} is required")
            .NotNull();

        }
        
    }
    
}