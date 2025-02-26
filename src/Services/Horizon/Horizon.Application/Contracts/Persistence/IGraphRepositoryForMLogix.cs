
using Horizon.Domain.MLogix;

namespace Horizon.Application.Contracts.Persistence
{
    public interface IGraphRepositoryForMLogix
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddMolecule(Molecule molecule);
        Task UpdateMolecule(Molecule molecule);
    }
}