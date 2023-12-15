
using FluentValidation;

namespace Target.Application.Features.Command.UpdateTargetAssociatedGenes
{
    public class UpdateTargetAssociatedGenesCommandValidator : AbstractValidator<UpdateTargetAssociatedGenesCommand>
    {
        public UpdateTargetAssociatedGenesCommandValidator()
        {
            
            RuleFor(t => t.AssociatedGenes)
               .Must(BeValidGuidKeyDictionary)
               .WithMessage("Each key in AssociatedGenes must be a valid GUID.");
        
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