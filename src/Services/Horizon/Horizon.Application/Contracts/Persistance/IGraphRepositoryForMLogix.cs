
using Horizon.Domain.MLogix;

namespace Horizon.Application.Contracts.Persistance
{
    public interface IGraphRepositoryForMLogix
    {
        Task CreateIndexesAsync();
        Task AddMolecule(Molecule molecule);
    }
}