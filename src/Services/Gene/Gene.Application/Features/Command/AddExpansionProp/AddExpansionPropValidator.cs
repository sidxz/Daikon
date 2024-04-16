
using FluentValidation;

namespace Gene.Application.Features.Command.AddExpansionProp
{
    public class AddExpansionPropValidator : AbstractValidator<AddExpansionPropCommand>
    {
        public AddExpansionPropValidator()
        {
            RuleFor(x => x.ExpansionType).NotEmpty();
            RuleFor(x => x.ExpansionValue).NotEmpty();
        }
    }

}