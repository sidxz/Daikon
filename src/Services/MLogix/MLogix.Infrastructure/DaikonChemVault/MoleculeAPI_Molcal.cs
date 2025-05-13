
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        // cluster
        public async Task<List<ClusterVM>> CalculateClusters(List<ClusterDTO> Molecules, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/molcal/cluster/";
            var clusters = await SendRequestAsync<List<ClusterVM>>(apiUrl, HttpMethod.Post, headers, Molecules);

            return clusters;
        }
    }
}