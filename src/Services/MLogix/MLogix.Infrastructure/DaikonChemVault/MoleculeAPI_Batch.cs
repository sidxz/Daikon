
using MediatR;
using Microsoft.Extensions.Logging;
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.RegisterMoleculeBatch;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<List<MoleculeBase>> RegisterBatch(List<RegisterMoleculeCommandWithRegId> registerMoleculeCommands, bool previewMode, IDictionary<string, string> headers)
        {
            
            var urlParams = new Dictionary<string, object>
            {
                { "preview_mode", previewMode }
            };
            string queryString = BuildQueryString(urlParams);

            string apiUrl = $"{_apiBaseUrl}/molecules/batch?{queryString}";
            
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