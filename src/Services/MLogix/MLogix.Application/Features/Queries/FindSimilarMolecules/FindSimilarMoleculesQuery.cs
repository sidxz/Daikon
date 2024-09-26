
using CQRS.Core.Query;
using MediatR;
using MLogix.Application.Features.Queries.Filters;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class FindSimilarMoleculesQuery : BaseQueryWithConditionFilters, IRequest<List<SimilarMoleculeVM>>
    {
        public string SMILES { get; set; }
        public double Threshold { get; set; } = 0.9;
        public int Limit { get; set; } = 100;

    }
}