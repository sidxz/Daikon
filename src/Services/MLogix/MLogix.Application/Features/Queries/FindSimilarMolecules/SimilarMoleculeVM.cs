using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Application.Features.Queries.GetMolecule;

namespace MLogix.Application.Features.Queries.FindSimilarMolecules
{
    public class SimilarMoleculeVM : MoleculeVM
    {
         public float Similarity { get; set; }
    }
}