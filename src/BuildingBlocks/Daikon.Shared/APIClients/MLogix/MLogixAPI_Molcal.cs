
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;

namespace Daikon.Shared.APIClients.MLogix
{
    public partial class MLogixAPI
    {
        public async Task<List<ClusterVM>> CalculateClusters(List<ClusterDTO> Molecules)
        {
            string apiUrl = $"{_apiBaseUrl}/molecule/cluster/";
            var clusters = await SendRequestAsync<List<ClusterVM>>(apiUrl, HttpMethod.Post, Molecules);
            return clusters;
        }
        
    }
}