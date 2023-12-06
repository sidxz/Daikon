using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gene.Application.Contracts.Persistence
{
    public interface IStrainRepository
    {
        Task CreateStrain(Domain.Entities.Strain strain);
        Task UpdateStrain(Domain.Entities.Strain strain);
        Task<Domain.Entities.Strain> ReadStrainById(Guid id);
        Task<Domain.Entities.Strain> ReadStrainByName(string name);
        Task DeleteStrain(Guid id);
        Task<List<Domain.Entities.Strain>> GetStrainsList();
    }
}