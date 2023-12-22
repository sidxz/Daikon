
using FluentValidation;
namespace Screen.Application.Features.Commands.NewScreenRun
{
    public class NewScreenRunCommandValidator : AbstractValidator<NewScreenRunCommand>
    {
        public NewScreenRunCommandValidator()
        {
            
            RuleFor(t => t.ScreenRunId)
            .NotEmpty().WithMessage("{ScreenRunId} is required")
            .NotNull();

            RuleFor(t => t.Id)
            .NotEmpty().WithMessage("{Id} is required");

        }
    }
}