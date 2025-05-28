
using FluentValidation;

namespace UserPreferences.Application.Features.Commands.SetTableDefaults
{
    public class SetTableDefaultsValidator : AbstractValidator<SetTableDefaultsCommand>
    {
        public SetTableDefaultsValidator()
        {
            RuleFor(command => command.TableType)
                .NotEmpty().WithMessage("Table type is required.");

            RuleFor(command => command.Columns)
                .NotEmpty().WithMessage("Columns are required.");
        }
    }
    
}
