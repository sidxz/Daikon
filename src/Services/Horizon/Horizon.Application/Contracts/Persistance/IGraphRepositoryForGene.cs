using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Domain.Genes;
using Horizon.Domain.Strains;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForGene
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddGeneToGraph(Gene gene);
        Task UpdateGeneOfGraph(Gene gene);
        Task AddStrainToGraph(Strain strain);
        Task UpdateStrainOfGraph(Strain strain);
    }
}