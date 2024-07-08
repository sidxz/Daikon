
using System.Net;
using MLogix.Application.DTOs.MolDbAPI;

namespace MLogix.Application.Contracts.Infrastructure
{
    public interface IMolDbAPIService
    {
        public Task<MoleculeDTO> RegisterCompound(string name, string initialCompoundStructure, IDictionary<string, string> headers);
        public Task<List<MoleculeDTO>> ListMolecules(IDictionary<string, string> headers);
        public Task<MoleculeDTO> GetMoleculeById(Guid id, IDictionary<string, string> headers);
        public Task<MoleculeDTO> GetMoleculeBySMILES(string smiles, IDictionary<string, string> headers);

        public Task<MoleculeDTO> FindExact(string smiles, IDictionary<string, string> headers);
        public Task<List<MoleculeDTO>> FindSimilar(string smiles, float similarityThreshold, int maxResults, IDictionary<string, string> headers);

        public Task<HttpStatusCode> DeleteMoleculeById(Guid id, IDictionary<string, string> headers);

    }
}