
using FluentValidation;
namespace Gene.Application.Features.Command.NewProteinProduction
{
    public class NewProteinProductionCommandValidator : AbstractValidator<NewProteinProductionCommand>
    {
        public NewProteinProductionCommandValidator()
        {
            RuleFor(e => e.Production)
            .NotEmpty().WithMessage("{Production} is required")
            .NotNull();

            RuleFor(e => e.GeneId)
            .NotEmpty().WithMessage("{GeneId} is required")
            .NotNull()
            .NotEqual(Guid.Empty).WithMessage("{GeneId} is required");
        }
    }
}