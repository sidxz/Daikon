
using FluentValidation;

namespace Target.Application.Features.Command.UpdateTarget
{
    public class UpdateTargetCommandValidator : AbstractValidator<UpdateTargetCommand>
    {
        public UpdateTargetCommandValidator()
        {
            
            RuleFor(t => t.StrainId)
            .NotEmpty().WithMessage("{StrainId} is required")
            .NotNull()
            .NotEqual(default(Guid));
            

            RuleFor(t => t.Name)
            .NotEmpty().WithMessage("{Name} is required")
            .NotNull();
        }
    }
}