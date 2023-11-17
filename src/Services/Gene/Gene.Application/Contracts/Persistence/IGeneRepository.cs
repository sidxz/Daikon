using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gene.Application.Contracts.Persistence
{
    public interface IGeneRepository
    {
        Task CreateGene(Domain.Entities.Gene gene);
        Task UpdateGene(Domain.Entities.Gene gene);
        Task ReadGene(Guid id);
        Task DeleteGene(Guid id);
        Task<List<Domain.Entities.Gene>> GetGenesList();
    }
}