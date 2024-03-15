using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Questionnaire.Application.Features.Queries.GetQuestionnaire.ById
{
    public class GetQuestionnaireValidator: AbstractValidator<GetQuestionnaireQuery>
    {
        public GetQuestionnaireValidator()
        {
            
        }
        
    }
}