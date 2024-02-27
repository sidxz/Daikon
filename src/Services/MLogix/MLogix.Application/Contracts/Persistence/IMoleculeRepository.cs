using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Domain.Entities;

namespace MLogix.Application.Contracts.Persistence
{
    public interface IMoleculeRepository
    {
        Task NewMolecule(Molecule molecule);
        Task UpdateMolecule(Molecule molecule);
        Task DeleteMolecule(Guid id);
        Task<Molecule> GetMoleculeById(Guid id);
        Task<Molecule> GetMoleculeByRegistrationId(Guid id);
        Task<Molecule> GetByName(string name);
        Task<List<Molecule>> GetAllMolecules();

    }
}