using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Domain.Genes;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForGene
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddGeneToGraph(Gene gene);
        Task AddStrainToGraph(string name, string strainId, string organism);
    }
}