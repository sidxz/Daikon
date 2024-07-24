using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Commands.DeleteToxicology
{
    public class DeleteToxicologyCommand : BaseCommand, IRequest<Unit>
    {
        public Guid TargetId { get; set; }
        public Guid ToxicologyId { get; set; }
    }
}