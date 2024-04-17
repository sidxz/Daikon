using FluentValidation;

namespace Comment.Application.Features.Commands.UpdateComment
{
    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            
            RuleFor(t => t.Topic)
               .NotEmpty().WithMessage("Topic is required");

            

        }
    }
    
}