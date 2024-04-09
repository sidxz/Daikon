
using FluentValidation;
namespace Project.Application.Features.Commands.UpdateProjectCompoundEvolution
{
    public class UpdateProjectCompoundEvolutionCommandValidator : AbstractValidator<UpdateProjectCompoundEvolutionCommand>
    {
        public UpdateProjectCompoundEvolutionCommandValidator()
        {
            RuleFor(t => t.Stage)
            .NotEmpty().WithMessage("Stage is required");

        }
    }
}