
using CQRS.Core.Query;
using Daikon.Shared.VM.MLogix;
using MediatR;
using MLogix.Application.Features.Queries.Filters;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.GetMolecule.ByName
{
    public class GetByNameQuery : BaseQuery, IRequest<List<MoleculeVM>>
    {
        public string Name { get; set; }
        public int Limit { get; set; } = 100;

    }
}