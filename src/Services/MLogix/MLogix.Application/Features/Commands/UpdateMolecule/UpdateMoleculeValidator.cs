
using FluentValidation;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeValidator : AbstractValidator<UpdateMoleculeCommand>
    {
        public UpdateMoleculeValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull();
        }
    }
}