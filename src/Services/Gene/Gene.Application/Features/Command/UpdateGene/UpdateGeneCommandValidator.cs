
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateGene
{
    public class UpdateGeneCommandValidator : AbstractValidator<UpdateGeneCommand>
    {
        public UpdateGeneCommandValidator()
        {
            RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{Id} is required")
            .NotNull();
        }
    }
}