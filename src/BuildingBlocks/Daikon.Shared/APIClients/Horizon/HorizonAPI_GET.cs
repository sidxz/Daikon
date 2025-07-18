using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Daikon.Shared.VM.Horizon;
using System.Net.Http;
using System.Net.Http.Json;

namespace Daikon.Shared.APIClients.Horizon
{
    public class CompoundRelationsMultipleRequest
    {
        public List<Guid> Ids { get; set; }
    }
    public partial class HorizonAPI
    {
        public async Task<CompoundRelationsMultipleVM> GetCompoundRelationsMultiple(List<Guid> ids, bool forceRefresh = false)
        {
            if (ids == null || !ids.Any())
            {
                _logger.LogError("Invalid ID list provided.");
                throw new ArgumentException("Invalid ID list.", nameof(ids));
            }

            var relations = new CompoundRelationsMultipleVM
            {
                Relations = new Dictionary<Guid, List<CompoundRelationsVM>>()
            };

            try
            {
                // Step 1: Check and retrieve from cache
                var missingIds = new List<Guid>();

                foreach (var id in ids)
                {
                    if (!forceRefresh && _cache.TryGetValue(id, out List<CompoundRelationsVM> cached))
                    {
                        relations.Relations[id] = cached;
                    }
                    else
                    {
                        missingIds.Add(id);
                    }
                }

                // Step 2: Fetch only missing IDs from API
                if (missingIds.Any())
                {
                    var apiUrl = $"{_apiBaseUrl}/horizon/find-molecule-relations?api-version=2.0";

                    var payload = new CompoundRelationsMultipleRequest
                    {
                        Ids = missingIds
                    };


                    var apiResponse = await SendRequestAsync<CompoundRelationsMultipleVM>(
                        apiUrl,
                        HttpMethod.Post,
                        payload
                    );

                    foreach (var kv in apiResponse.Relations)
                    {
                        _cache.Set(kv.Key, kv.Value, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                        relations.Relations[kv.Key] = kv.Value;
                    }
                }

                return relations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching compound relations for multiple IDs.");
                throw new ApplicationException("Unable to retrieve compound relations. Please try again later.");
            }
        }
    }
}
