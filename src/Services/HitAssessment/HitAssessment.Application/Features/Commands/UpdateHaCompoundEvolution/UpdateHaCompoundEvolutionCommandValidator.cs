
using FluentValidation;
namespace HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution
{
    public class UpdateHaCompoundEvolutionCommandValidator : AbstractValidator<UpdateHaCompoundEvolutionCommand>
    {
        public UpdateHaCompoundEvolutionCommandValidator()
        {


            RuleFor(t => t.EvolutionDate)
            .NotEmpty().WithMessage("Evolution Date is required");

        }
    }
}