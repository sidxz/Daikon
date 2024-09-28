
using Microsoft.Extensions.Logging;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMolecule;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<MoleculeBase> Register(RegisterMoleculeCommand registerMoleculeCommand, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molecules/";
            var molecule = await SendRequestAsync<MoleculeBase>(apiUrl, HttpMethod.Post, headers, registerMoleculeCommand);
            return molecule;
        }
    }
}