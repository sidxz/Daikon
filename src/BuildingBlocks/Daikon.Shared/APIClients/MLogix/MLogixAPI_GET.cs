
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.APIClients.MLogix
{
    public partial class MLogixAPI : IMLogixAPI
    {
        public async Task<MoleculeVM> GetMoleculeById(Guid Id)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/{Id}";
            var molecule = await SendRequestAsync<MoleculeVM>(apiUrl, HttpMethod.Get);
            return molecule;
        }
    }
}