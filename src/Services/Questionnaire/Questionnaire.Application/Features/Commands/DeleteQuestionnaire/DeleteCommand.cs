using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Questionnaire.Application.Features.Commands.DeleteQuestionnaire
{
    public class DeleteCommand : BaseCommand, IRequest<Unit>
    {
        public string Name { get; set; }

    }
}