using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.GenerateParentBatch
{
    public class GenerateParentBatchCommand : BaseCommand, IRequest<Unit>
    {

    }
}