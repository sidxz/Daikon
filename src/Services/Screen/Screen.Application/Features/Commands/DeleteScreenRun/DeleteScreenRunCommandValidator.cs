using FluentValidation;

namespace Screen.Application.Features.Commands.DeleteScreenRun
{
    public class DeleteScreenRunCommandValidator : AbstractValidator<DeleteScreenRunCommand>
    {
        public DeleteScreenRunCommandValidator()
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