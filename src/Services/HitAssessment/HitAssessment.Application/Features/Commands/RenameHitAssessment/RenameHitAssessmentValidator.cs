using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace HitAssessment.Application.Features.Commands.RenameHitAssessment
{
    public class RenameHitAssessmentValidator : AbstractValidator<RenameHitAssessmentCommand>
    {
        public RenameHitAssessmentValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }

}