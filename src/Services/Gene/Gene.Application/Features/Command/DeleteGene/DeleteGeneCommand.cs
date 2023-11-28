using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace Gene.Application.Features.Command.DeleteGene
{
    public class DeleteGeneCommand : BaseCommand, IRequest<Unit>
    {
        
    }
}