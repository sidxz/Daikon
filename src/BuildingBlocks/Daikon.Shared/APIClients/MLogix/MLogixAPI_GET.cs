
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


        public async Task<List<MoleculeVM>> GetMoleculesByIds(List<Guid> IDs)
        {
            if (IDs == null || !IDs.Any())
            {
                return new List<MoleculeVM>();
            }
            var body = new
            {
                IDs = IDs
            };
            var apiUrl = $"{_apiBaseUrl}/molecule/by-ids";
            var molecules = await SendRequestAsync<List<MoleculeVM>>(apiUrl, HttpMethod.Post, body);
            return molecules;
        }

        public Task<List<MoleculeVM>> GetRecentDisclosures(DateTime? startDate, DateTime? endDate)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/recent-disclosure?startDate={startDate}&endDate={endDate}";
            return SendRequestAsync<List<MoleculeVM>>(apiUrl, HttpMethod.Get);
        }

        
    }
}