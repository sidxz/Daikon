using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Target.Application.Features.Commands.RejectTarget
{
    public class RejectTargetCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}