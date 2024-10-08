
using MediatR;
using Microsoft.Extensions.Logging;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMolecule;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<List<MoleculeBase>> RegisterBatch(List<RegisterMoleculeCommandWithRegId> registerMoleculeCommands, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molecules/batch";
            var molecules = await SendRequestAsync<List<MoleculeBase>>(apiUrl, HttpMethod.Post, headers, registerMoleculeCommands);
            _logger.LogDebug("Molecules registered count: {Count}", molecules.Count);
            return molecules;
        }

        public async Task<Unit> BatchCreateParents(IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molecules/batch-create-parents";
            var molecules = await SendRequestAsync<MoleculeBase>(apiUrl, HttpMethod.Post, headers);
            return Unit.Value;
        }
    }
}