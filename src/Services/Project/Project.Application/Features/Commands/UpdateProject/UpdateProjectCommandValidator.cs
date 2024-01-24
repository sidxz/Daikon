
using FluentValidation;

namespace Project.Application.Features.Commands.UpdateProject
{
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            
            RuleFor(t => t.StrainId)
            .NotEmpty().WithMessage("{StrainId} is required");

            RuleFor(t => t.AssociatedHitIds)
               .Must(BeValidGuidKeyDictionary)
               .WithMessage("Each key in AssociatedHitIds must be a valid GUID.");

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