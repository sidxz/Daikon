using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Questionnaire.Application.Features.Queries.GetQuestionnaire.ById
{
    public class GetQuestionnaireQuery : IRequest<Domain.Entities.Questionnaire>
    {
        public Guid Id { get; set; }
    }
}