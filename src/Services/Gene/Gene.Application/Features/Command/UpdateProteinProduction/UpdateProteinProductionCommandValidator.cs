
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateProteinProduction
{
    public class UpdateProteinProductionCommandValidator : AbstractValidator<UpdateProteinProductionCommand>
    {
        public UpdateProteinProductionCommandValidator()
        {
             RuleFor(e => e.Production)
            .NotEmpty().WithMessage("{Production} is required")
            .NotNull();
        }
    }
}