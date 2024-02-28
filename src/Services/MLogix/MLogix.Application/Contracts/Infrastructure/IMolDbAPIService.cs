
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Application.Contracts.Infrastructure
{
    public interface IMolDbAPIService
    {
        public Task<MoleculeDTO> RegisterCompound(string name, string initialCompoundStructure);
        public Task<List<MoleculeDTO>> ListMolecules();
        public Task<MoleculeDTO> GetMoleculeById(Guid id);
        public Task<MoleculeDTO> GetMoleculeBySMILES(string smiles);

        public Task<MoleculeDTO> FindExact(string smiles);
        public Task<List<MoleculeDTO>> FindSimilar(string smiles, float similarityThreshold, int maxResults);

    }
}