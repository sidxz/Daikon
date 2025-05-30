using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Command;
using Daikon.Shared.VM.MLogix;
using MediatR;

namespace MLogix.Application.Features.Commands.DiscloseMolecule
{
    public class DiscloseMoleculeBatchCommand : BaseCommand, IRequest<IList<MoleculeVM>>
    {
        public List<DiscloseMoleculeCommand> Molecules { get; set; } = [];
    }
}