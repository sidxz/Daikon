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
        Task<Domain.Entities.Gene> ReadGeneById(Guid id);
        Task<Domain.Entities.Gene> ReadGeneByAccession(string accessionNumber);
        Task DeleteGene(Guid id);
        Task<List<Domain.Entities.Gene>> GetGenesList();
        Task<List<Domain.Entities.Gene>> GetGenesListByStrainId(Guid strainId);
        Task<Domain.EntityRevisions.GeneRevision> GetGeneRevisions(Guid id);
    }
}