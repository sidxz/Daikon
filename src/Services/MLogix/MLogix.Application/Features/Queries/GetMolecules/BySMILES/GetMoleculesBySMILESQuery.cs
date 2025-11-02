using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Daikon.Shared.VM.MLogix;
using MediatR;

namespace MLogix.Application.Features.Queries.GetMolecules.BySMILES
{
    public class GetMoleculesBySMILESQuery : BaseQuery, IRequest<List<MoleculeVM>>
    {
        public List<string> SMILES { get; set; }
    }
}