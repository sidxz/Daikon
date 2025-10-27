using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using MediatR;

namespace Horizon.Application.Features.Command.Patches.HandleDuplicates
{
    public class HandleDuplicatesMolRelationsCommand : BaseQuery, IRequest<Unit>
    {

    }
}