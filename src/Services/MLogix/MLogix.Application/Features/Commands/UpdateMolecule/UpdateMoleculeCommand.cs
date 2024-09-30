using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using MediatR;

namespace MLogix.Application.Features.Commands.UpdateMolecule
{
    public class UpdateMoleculeCommand : BaseCommand, IRequest<UpdateMoleculeResponseDTO>
    {
        public string? Name { get; set; }
        public required string RequestedSMILES { get; set; }
        public string? Synonyms { get; set; }
    }
}