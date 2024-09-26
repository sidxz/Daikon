
using CQRS.Core.Query;
using MediatR;
using MLogix.Application.Features.Queries.Filters;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.FindSubstructures
{
    public class FindSubstructuresQuery : BaseQueryWithConditionFilters, IRequest<List<MoleculeVM>>
    {
        public string SMILES { get; set; }
        public int Limit { get; set; } = 100;

    }
}