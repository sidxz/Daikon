using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.UpdateTPQ
{
    public class UpdateTPQValidator : AbstractValidator<UpdateTPQCommand>
    {
        public UpdateTPQValidator()
        {
            RuleFor(x => x.RequestedTargetName).NotEmpty();
            RuleFor(x => x.RequestedAssociatedGenes).NotEmpty();
            RuleFor(x => x.Response).NotEmpty();
        }
    }

}