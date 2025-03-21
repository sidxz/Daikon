using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using Daikon.Shared.VM.MLogix;
using MediatR;

namespace MLogix.Application.Features.Commands.DiscloseMolecule
{
    public class DiscloseMoleculeCommand : BaseCommand, IRequest<MoleculeVM>
    {
        public required string Name { get; set; }
        public required string RequestedSMILES { get; set; }
        public string? Synonyms { get; set; }
    }
}