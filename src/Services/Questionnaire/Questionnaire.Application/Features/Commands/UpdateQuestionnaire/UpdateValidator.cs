using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Questionnaire.Application.Features.Commands.UpdateQuestionnaire
{
    public class UpdateValidator : AbstractValidator<UpdateCommand>
    {
        public UpdateValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }

}