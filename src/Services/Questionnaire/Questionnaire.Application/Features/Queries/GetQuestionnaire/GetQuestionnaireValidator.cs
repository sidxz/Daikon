using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Questionnaire.Application.Features.Queries.GetQuestionnaire
{
    public class GetQuestionnaireValidator: AbstractValidator<GetQuestionnaireQuery>
    {
        public GetQuestionnaireValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();
        }
        
    }
}