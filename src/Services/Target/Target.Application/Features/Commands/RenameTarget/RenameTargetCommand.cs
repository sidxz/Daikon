using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Commands.RenameTarget
{
    public class RenameTargetCommand : BaseCommand, IRequest<Unit>
    {
        public Guid StrainId { get; set; }
        public string Name { get; set; }
    }
}