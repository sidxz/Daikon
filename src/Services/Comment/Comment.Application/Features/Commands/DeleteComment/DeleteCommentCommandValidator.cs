using FluentValidation;

namespace Comment.Application.Features.Commands.DeleteComment
{
    public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentCommandValidator()
        {
            
            RuleFor(t => t.ResourceId)
            .NotEmpty().WithMessage("{ResourceId} is required");

            RuleFor(t => t.Topic)
               .NotEmpty().WithMessage("{Topic} is required");

            
        }
    }
}