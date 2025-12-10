using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.VM.MLogix;
using MediatR;

namespace MLogix.Application.Features.Queries.GetUndisclosedMolecules.ByName
{
    public class GetUndisclosedMoleculesByNameQuery : IRequest<List<MoleculeVM>>
    {
        public required List<string> Names { get; set; } = [];
    }
}