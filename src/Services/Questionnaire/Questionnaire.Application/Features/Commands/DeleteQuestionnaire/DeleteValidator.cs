using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Questionnaire.Application.Features.Commands.DeleteQuestionnaire
{
    public class DeleteValidator : AbstractValidator<DeleteCommand>
    {
        public DeleteValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}