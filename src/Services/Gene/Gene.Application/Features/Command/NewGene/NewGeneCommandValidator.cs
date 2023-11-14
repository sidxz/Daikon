
using FluentValidation;
namespace Gene.Application.Features.Command.NewGene
{
    public class NewGeneCommandValidator : AbstractValidator<NewGeneCommand>
    {
        public NewGeneCommandValidator()
        {
            RuleFor(p => p.AccessionNumber)
            .NotEmpty().WithMessage("{AccessionNumber} is required")
            .NotNull();
        }
    }
}