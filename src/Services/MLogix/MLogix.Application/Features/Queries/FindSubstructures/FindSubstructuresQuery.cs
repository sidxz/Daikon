
using Daikon.Shared.VM.MLogix;
using MediatR;
using MLogix.Application.Features.Queries.Filters;

namespace MLogix.Application.Features.Queries.FindSubstructures
{
    public class FindSubstructuresQuery : BaseQueryWithConditionFilters, IRequest<List<MoleculeVM>>
    {
        public string SMILES { get; set; }
        public int Limit { get; set; } = 100;

    }
}