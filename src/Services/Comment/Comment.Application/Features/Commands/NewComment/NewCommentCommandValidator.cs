
using FluentValidation;

namespace Comment.Application.Features.Commands.NewComment
{
    public class NewCommentCommandValidator : AbstractValidator<NewCommentCommand>
    {
        public NewCommentCommandValidator()
        {
            
            RuleFor(t => t.Topic)
            .NotEmpty().WithMessage("Topic is required")
            .NotNull();

        }
        
    }
    
}