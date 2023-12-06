using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteStrain
{
    public class DeleteStrainCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}