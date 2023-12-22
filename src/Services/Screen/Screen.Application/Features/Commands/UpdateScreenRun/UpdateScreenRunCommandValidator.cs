using FluentValidation;

namespace Screen.Application.Features.Commands.UpdateScreenRun
{
    public class UpdateScreenRunCommandValidator : AbstractValidator<UpdateScreenRunCommand>
    {
        public UpdateScreenRunCommandValidator()
        {
            RuleFor(t => t.Id)
                .NotEmpty().WithMessage("{ScreenId} is required")
                .NotNull();

            RuleFor(t => t.ScreenRunId)
                .NotEmpty().WithMessage("{ScreenRunId} is required")
                .NotNull();
        }
    }
}