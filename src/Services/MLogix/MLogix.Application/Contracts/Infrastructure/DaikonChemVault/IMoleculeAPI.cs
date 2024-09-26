
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Queries.FindSimilarMolecules;
using MLogix.Application.Features.Queries.FindSubstructures;

namespace MLogix.Application.Contracts.Infrastructure.DaikonChemVault
{
    public interface IMoleculeAPI
    {
        public Task<MoleculeBase> GetMoleculeById(Guid RegistrationId, IDictionary<string, string> headers);
        public Task<MoleculeBase> GetMoleculeBySMILES(string smiles, IDictionary<string, string> headers);
        public Task<List<SimilarMolecule>> FindSimilar(FindSimilarMoleculesQuery query, IDictionary<string, string> headers);
        public Task<List<MoleculeBase>> FindSubstructure(FindSubstructuresQuery query, IDictionary<string, string> headers);
    }
}