using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Target.Application.Features.Commands.ApproveTarget
{
    public class ApproveTargetValidator : AbstractValidator<ApproveTargetCommand>
    {
        public ApproveTargetValidator()
        {
            RuleFor(x => x.TPQId).NotEmpty();
            RuleFor(x => x.TargetName).NotEmpty();
            RuleFor(x => x.AssociatedGenes).NotEmpty();
        }
        
    }
}