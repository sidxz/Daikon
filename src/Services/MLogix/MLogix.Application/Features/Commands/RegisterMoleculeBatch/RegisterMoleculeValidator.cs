

using FluentValidation;
namespace MLogix.Application.Features.Commands.RegisterMoleculeBatch
{
    public class RegisterMoleculeValidator : AbstractValidator<RegisterMoleculeCommand>
    {
        public RegisterMoleculeValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull();
        }

    }
}