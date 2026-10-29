
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.APIClients.MLogix
{
    public partial class MLogixAPI
    {
        // MLogix registers only through its batch route (single-item batches are supported). Null when MLogix returns
        // no row: the name already belongs to another structure, or the structure can't be linked to a molecule.
        public async Task<MoleculeVM?> RegisterMolecule(RegisterMoleculeDTO request)
        {
            var molecules = await RegisterBatch(new List<RegisterMoleculeDTO> { request });
            return molecules?.FirstOrDefault();
        }

        public async Task<List<MoleculeVM>> RegisterBatch(List<RegisterMoleculeDTO> request)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/batch/";
            var molecules = await SendRequestAsync<List<MoleculeVM>>(apiUrl, HttpMethod.Post, request);
            return molecules;
        }
    }
}