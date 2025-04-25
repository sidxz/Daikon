
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.APIClients.MLogix
{
    public partial class MLogixAPI
    {
        public async Task<MoleculeVM> RegisterMolecule(RegisterMoleculeDTO request)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/";
            var molecule = await SendRequestAsync<MoleculeVM>(apiUrl, HttpMethod.Post, request);
            return molecule;
        }

        public async Task<List<MoleculeVM>> RegisterBatch(List<RegisterMoleculeDTO> request)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/batch/";
            var molecules = await SendRequestAsync<List<MoleculeVM>>(apiUrl, HttpMethod.Post, request);
            return molecules;
        }
    }
}