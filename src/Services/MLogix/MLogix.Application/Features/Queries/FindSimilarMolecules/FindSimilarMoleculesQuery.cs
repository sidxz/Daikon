
using CQRS.Core.Query;
using MediatR;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class FindSimilarMoleculesQuery : BaseQuery, IRequest<List<MoleculeVM>>
    {
        public string SMILES { get; set; }
        public double SimilarityThreshold { get; set; }
        public int MaxResults { get; set; }
    }
}