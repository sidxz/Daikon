
using FluentValidation;
namespace HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution
{
    public class UpdateHaCompoundEvolutionCommandValidator : AbstractValidator<UpdateHaCompoundEvolutionCommand>
    {
        public UpdateHaCompoundEvolutionCommandValidator()
        {
            

            RuleFor(t => t.CompoundStructureSMILES)
            .NotEmpty().WithMessage("SMILES is required");

        }
    }
}