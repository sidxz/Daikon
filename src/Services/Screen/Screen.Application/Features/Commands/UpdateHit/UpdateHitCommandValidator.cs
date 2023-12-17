
using FluentValidation;

namespace Screen.Application.Features.Commands.UpdateHit
{
    public class UpdateHitCommandValidator : AbstractValidator<UpdateHitCommand>
    {
        public UpdateHitCommandValidator()
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