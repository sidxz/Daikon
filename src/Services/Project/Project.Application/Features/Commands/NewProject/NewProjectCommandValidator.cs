
using FluentValidation;

namespace Project.Application.Features.Commands.NewProject
{
    public class NewProjectCommandValidator : AbstractValidator<NewProjectCommand>
    {
        public NewProjectCommandValidator()
        {
            
            RuleFor(t => t.Name)
            .NotEmpty().WithMessage("{Name} is required")
            .NotNull();

            RuleFor(t => t.StrainId)
            .NotEmpty().WithMessage("{StrainId} is required");
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