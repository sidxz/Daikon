using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Questionnaire.Application.Features.Commands.CreateQuestionnaire
{
    public class CreateValidator : AbstractValidator<CreateCommand>
    {
        public CreateValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}