
using FluentValidation;


namespace Screen.Application.Features.Commands.UpdateHitCollection
{
    public class UpdateHitCollectionCommandValidator : AbstractValidator<UpdateHitCollectionCommand>

    {
        public UpdateHitCollectionCommandValidator()
        {

            RuleFor(t => t.HitCollectionType)
                .NotEmpty().WithMessage("{HitCollectionType} is required")
                .NotNull();

        }

    }
}