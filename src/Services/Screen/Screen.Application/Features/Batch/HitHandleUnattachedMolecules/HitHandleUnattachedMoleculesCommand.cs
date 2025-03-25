using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Screen.Application.Features.Batch.HitHandleUnattachedMolecules
{
    public class HitHandleUnattachedMoleculesCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}