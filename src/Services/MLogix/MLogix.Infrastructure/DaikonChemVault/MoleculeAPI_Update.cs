
using MLogix.Application.DTOs.DaikonChemVault;
using MLogix.Application.Features.Commands.UpdateMolecule;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<MoleculeBase> Update(Guid RegistrationId, UpdateMoleculeCommand command, IDictionary<string, string> headers)
        {
            command.Id = RegistrationId;
            string apiUrl = $"{_apiBaseUrl}/molecules/{RegistrationId}";
            var molecule = await SendRequestAsync<MoleculeBase>(apiUrl, HttpMethod.Put, headers, command);
            return molecule;
        }
    }
}