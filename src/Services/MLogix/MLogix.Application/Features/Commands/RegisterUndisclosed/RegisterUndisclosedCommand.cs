using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.RegisterUndisclosed
{
    public class RegisterUndisclosedCommand : BaseCommand, IRequest<RegisterUndisclosedDTO>
    {
        public required string Name { get; set; }
        public bool ChemVaultCheck { get; set; } = true;
        
    }
}