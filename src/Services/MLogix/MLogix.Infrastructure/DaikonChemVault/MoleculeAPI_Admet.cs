
using MLogix.Application.DTOs.DaikonChemVault;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI
    {
        public async Task<List<AdmetCalcResult>> PredictAdmet(List<string> smilesList, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/admet/predict";
            var results = await SendRequestAsync<List<AdmetCalcResult>>(apiUrl, HttpMethod.Post, headers, smilesList);
            return results;
        }

        public async Task<AdmetVM> GetAdmetByMoleculeId(Guid RegistrationId, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/admet/{RegistrationId}";
            var prediction = await SendRequestAsync<AdmetVM>(apiUrl, HttpMethod.Get, headers);
            return prediction;
        }

        public async Task<List<AdmetVM>> GetAdmetByMoleculeIds(List<Guid> RegistrationIds, IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/admet/by-ids";
            var predictions = await SendRequestAsync<List<AdmetVM>>(apiUrl, HttpMethod.Post, headers, RegistrationIds);
            return predictions;
        }

        public async Task<AdmetBackfillTriggerVM> TriggerAdmetBackfill(int chunkSize, int? limit, bool includeErrors, IDictionary<string, string> headers)
        {
            var queryParams = new Dictionary<string, object>
            {
                { "chunk_size", chunkSize },
                { "limit", limit },
                { "include_errors", includeErrors }
            };
            string queryString = BuildQueryString(queryParams);
            string apiUrl = $"{_apiBaseUrl}/admet/backfill?{queryString}";
            var result = await SendRequestAsync<AdmetBackfillTriggerVM>(apiUrl, HttpMethod.Post, headers);
            return result;
        }

        public async Task<AdmetBackfillCountsVM> GetAdmetBackfillStatus(IDictionary<string, string> headers)
        {
            string apiUrl = $"{_apiBaseUrl}/admet/backfill/status";
            var counts = await SendRequestAsync<AdmetBackfillCountsVM>(apiUrl, HttpMethod.Get, headers);
            return counts;
        }
    }
}
