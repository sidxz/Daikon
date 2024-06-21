
using FluentValidation;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeValidator : AbstractValidator<UpdateMoleculeCommand>
    {
        public UpdateMoleculeValidator()
        {
            RuleFor(p => p.RequestedSMILES)
                .NotEmpty().WithMessage("SMILES is required.")
                .NotNull();
        }
    }
}