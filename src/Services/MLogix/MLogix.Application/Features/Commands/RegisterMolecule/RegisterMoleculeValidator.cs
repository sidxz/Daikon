

using FluentValidation;

namespace MLogix.Application.Features.Commands.RegisterMolecule
{
    public class RegisterMoleculeValidator : AbstractValidator<RegisterMoleculeCommand>
    {
        public RegisterMoleculeValidator()
        {
            
            RuleFor(p => p.SMILES)
                .NotEmpty().WithMessage("SMILES is required.")
                .NotNull();
        }

    }
}