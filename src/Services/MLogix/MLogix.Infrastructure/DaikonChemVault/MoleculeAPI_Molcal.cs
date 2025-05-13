
using Daikon.Shared.DTO.MLogix;
using Daikon.Shared.VM.MLogix;
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        // cluster
        public async Task<List<ClusterVM>> CalculateClusters(List<ClusterDTO> Molecules, double CutOff, IDictionary<string, string> headers)
        {
            // if cutOff is 0 or is null, set it to 0.7
            if (CutOff == 0)
            {
                CutOff = 0.7f;
            }
            string apiUrl = $"{_apiBaseUrl}/molcal/cluster?cutoff={CutOff}";
            var clusters = await SendRequestAsync<List<ClusterVM>>(apiUrl, HttpMethod.Post, headers, Molecules);

            return clusters;
        }
    }
}