using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Questionnaire.Application.Features.Queries.ListQuestionnaires
{
    public class ListQuestionnaireQuery : IRequest<List<Domain.Entities.Questionnaire>>
    {

    }
}