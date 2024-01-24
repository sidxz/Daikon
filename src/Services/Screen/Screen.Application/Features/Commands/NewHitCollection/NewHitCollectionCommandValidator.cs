
using FluentValidation;

namespace Screen.Application.Features.Commands.NewHitCollection
{
    public class NewHitCollectionCommandValidator : AbstractValidator<NewHitCollectionCommand>
    {

        public NewHitCollectionCommandValidator()
        {
            
            
            RuleFor(t => t.ScreenId)
            .NotEmpty().WithMessage("{ScreenId} is required")
            .NotNull();

            RuleFor(t => t.Name)
            .NotEmpty().WithMessage("{Name} is required")
            .NotNull();

            RuleFor(t => t.HitCollectionType)
            .NotEmpty().WithMessage("{HitCollectionType} is required")
            .NotNull();

        }

        
    }
}