
using Daikon.Shared.VM.MLogix;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class SimilarMoleculeVM : MoleculeVM
    {
         public float Similarity { get; set; }
    }
}