
using FluentValidation;


namespace Screen.Application.Features.Commands.RenameHitCollection
{
    public class RenameHitCollectionCommandValidator : AbstractValidator<RenameHitCollectionCommand>

    {
        public RenameHitCollectionCommandValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("{Name} is required")
                .NotNull();
        }

    }
}