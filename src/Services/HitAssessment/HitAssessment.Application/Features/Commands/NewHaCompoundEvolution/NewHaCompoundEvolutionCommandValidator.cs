
using FluentValidation;
namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionCommandValidator : AbstractValidator<NewHaCompoundEvolutionCommand>
    {
        public NewHaCompoundEvolutionCommandValidator()
        {

            RuleFor(t => t.RequestedSMILES)
            .NotEmpty().WithMessage("SMILES is required");

        }
    }
}