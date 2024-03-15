using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;
using Questionnaire.Domain.Entities;

namespace Questionnaire.Application.Features.Commands.CreateQuestionnaire
{
    public class CreateCommand : BaseCommand, IRequest<Domain.Entities.Questionnaire>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Version { get; set; }
        public List<Question>? Questions { get; set; }
    }
}