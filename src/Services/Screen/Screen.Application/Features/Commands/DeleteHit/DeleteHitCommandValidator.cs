
using FluentValidation;

namespace Screen.Application.Features.Commands.DeleteHit
{
    public class DeleteHitCommandValidator : AbstractValidator<DeleteHitCommand>
    {
        public DeleteHitCommandValidator()
        {
            RuleFor(t => t.HitCollectionId)
                .NotEmpty().WithMessage("{HitCollectionId} is required")
                .NotNull();

            RuleFor(t => t.HitId)
                .NotEmpty().WithMessage("{HitId} is required")
                .NotNull();
        }

    }
}