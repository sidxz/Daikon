using MLogix.Domain.Entities;

namespace MLogix.Application.Contracts.Persistence
{
    public interface IMoleculeRepository
    {
        Task NewMolecule(Molecule molecule);
        Task UpdateMolecule(Molecule molecule);
        Task DeleteMolecule(Guid id);
        Task<Molecule> GetMoleculeById(Guid id);
        Task<List<Molecule>> GetMoleculesByIds(List<Guid> ids);
        Task<Molecule> GetMoleculeByRegistrationId(Guid id);
        Task<Molecule> GetByName(string name);
        Task<List<Molecule>> GetAllMolecules();
        Task<List<Molecule>> GetDisclosedMolecules(DateTime? startDate = null, DateTime? endDate = null);

    }
}