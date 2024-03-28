
using FluentValidation;
namespace HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution
{
    public class UpdateHaCompoundEvolutionCommandValidator : AbstractValidator<UpdateHaCompoundEvolutionCommand>
    {
        public UpdateHaCompoundEvolutionCommandValidator()
        {
            

            RuleFor(t => t.RequestedSMILES)
            .NotEmpty().WithMessage("SMILES is required");

        }
    }
}