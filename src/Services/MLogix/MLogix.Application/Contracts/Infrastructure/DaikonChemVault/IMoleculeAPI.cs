using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Application.Contracts.Infrastructure.DaikonChemVault
{
    public interface IMoleculeAPI
    {
        public Task<MoleculeBase> GetMoleculeById(Guid RegistrationId, IDictionary<string, string> headers);
        public Task<MoleculeBase> GetMoleculeBySMILES(string smiles, IDictionary<string, string> headers);
        public Task<List<SimilarMolecule>> FindSimilar(string smiles, float similarityThreshold, int maxResults, IDictionary<string, string> headers);
    }
}