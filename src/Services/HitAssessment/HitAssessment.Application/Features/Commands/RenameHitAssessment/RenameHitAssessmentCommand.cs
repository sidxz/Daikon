using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace HitAssessment.Application.Features.Commands.RenameHitAssessment
{
    public class RenameHitAssessmentCommand : BaseCommand, IRequest<Unit>
    {
        public string Name { get; set; }
    }
}