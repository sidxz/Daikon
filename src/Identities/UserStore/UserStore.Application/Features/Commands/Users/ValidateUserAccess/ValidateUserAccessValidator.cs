
using FluentValidation;

namespace UserStore.Application.Features.Commands.Users.ValidateUserAccess
{
    public class ValidateUserAccessValidator : AbstractValidator<ValidateUserAccessCommand>
    {

        public ValidateUserAccessValidator()
        {

            RuleFor(command => command)
                 .Must(command => !(string.IsNullOrEmpty(command.Email) && string.IsNullOrEmpty(command.OIDCSub) && command.EntraObjectId == Guid.Empty))
                 .WithMessage("Either Email, OIDCSub or EntraObjectId must be provided.");

        }
    }
    
}