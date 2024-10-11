using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Query;
using Daikon.Shared.VM.MLogix;
using MediatR;

namespace MLogix.Application.Features.Queries.GetMolecules.ByIDs
{
    public class GetMoleculeByIDsQuery : BaseQuery, IRequest<List<MoleculeVM>>
    {
        public required List<Guid> IDs { get; set; }
    }
}