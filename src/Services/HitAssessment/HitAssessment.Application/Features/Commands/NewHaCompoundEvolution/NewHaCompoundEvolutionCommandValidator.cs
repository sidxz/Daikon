
using FluentValidation;
namespace HitAssessment.Application.Features.Commands.NewHaCompoundEvolution
{
    public class NewHaCompoundEvolutionCommandValidator : AbstractValidator<NewHaCompoundEvolutionCommand>
    {
        public NewHaCompoundEvolutionCommandValidator()
        {
            
            RuleFor(t => t.HitAssessmentId)
            .NotEmpty().WithMessage("{HitAssessmentId} is required")
            .NotNull();

            RuleFor(t => t.CompoundStructureSMILES)
            .NotEmpty().WithMessage("SMILES is required");

        }
    }
}