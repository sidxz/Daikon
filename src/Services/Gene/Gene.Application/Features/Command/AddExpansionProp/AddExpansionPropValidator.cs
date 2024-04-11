using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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