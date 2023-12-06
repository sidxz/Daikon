using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepository
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddGeneToGraph(string accessionNumber, string name, string function, string product, string functionalCategory);
    }
}