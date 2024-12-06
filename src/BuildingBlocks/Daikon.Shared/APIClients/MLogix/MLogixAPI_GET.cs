
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

        public async Task<MoleculeVM> GetMoleculeBySmiles(string SMILES)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/similar?SMILES={Uri.EscapeDataString(SMILES)}&Threshold=1&Limit=1";
            var result = await SendRequestAsync<List<MoleculeVM>>(apiUrl, HttpMethod.Get);

            if (result != null && result.Count > 0)
            {
                return result.First();
            }

            return null;
        }


        public async Task<List<MoleculeVM>> GetMoleculesByIds(List<Guid> Ids)
        {
            string idsQuery = string.Join("&", Ids.Select(id => $"ids={id}"));
            string apiUrl = $"{_apiBaseUrl}/molecule/by-ids?{idsQuery}";
            var molecules = await SendRequestAsync<List<MoleculeVM>>(apiUrl, HttpMethod.Get);
            return molecules;
        }
    }
}