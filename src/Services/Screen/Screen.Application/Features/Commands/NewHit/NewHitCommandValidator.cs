
using FluentValidation;

namespace Screen.Application.Features.Commands.NewHit
{
    public class NewHitCommandValidator : AbstractValidator<NewHitCommand>
    {
        public NewHitCommandValidator()
        {
            RuleFor(t => t.HitCollectionId)
                .NotEmpty().WithMessage("{HitCollectionId} is required")
                .NotNull();

            RuleFor(t => t.HitId)
                .NotEmpty().WithMessage("{HitId} is required")
                .NotNull();
        }

        private bool BeValidGuidKeyDictionary(Dictionary<string, string>? dictionary)
        {
            if (dictionary == null) return true; // Assuming null is valid. If not, return false.

            foreach (var key in dictionary.Keys)
            {
                if (!Guid.TryParse(key, out _))
                {
                    return false;
                }
            }

            return true;
        }
    }
}