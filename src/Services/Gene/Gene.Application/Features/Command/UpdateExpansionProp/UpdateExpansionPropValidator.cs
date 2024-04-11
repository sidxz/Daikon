
using FluentValidation;

namespace Gene.Application.Features.Command.UpdateExpansionProp
{
    public class UpdateExpansionPropValidator : AbstractValidator<UpdateExpansionPropCommand>
    {
        public UpdateExpansionPropValidator()
        {
            RuleFor(x => x.ExpansionPropId).NotEmpty();
            RuleFor(x => x.ExpansionValue).NotEmpty();
        }
    }

}