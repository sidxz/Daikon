
using Horizon.Domain.MLogix;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForMLogix
    {
        Task CreateIndexesAsync();
        Task CreateConstraintsAsync();
        Task AddMolecule(Molecule molecule);
    }
}