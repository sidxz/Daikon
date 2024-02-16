
using FluentValidation;

namespace UserStore.Application.Features.Commands.Orgs.UpdateOrg
{
    public class UpdateOrgValidator : AbstractValidator<UpdateOrgCommand>
    {
        public UpdateOrgValidator()
        {
           
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(command => command.Alias)
                .NotEmpty().WithMessage("Alias is required.")
                .MaximumLength(50).WithMessage("Alias must not exceed 50 characters.");
        }
    }
    
}