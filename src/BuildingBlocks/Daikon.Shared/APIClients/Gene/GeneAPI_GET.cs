
using Daikon.Shared.VM.Gene;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Daikon.Shared.APIClients.Gene
{
    public partial class GeneAPI
    {
        public async Task<GeneLiteVM> GetBasicById(Guid id, bool forceRefresh = false)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid ID provided.");
                throw new ArgumentException("Invalid ID.", nameof(id));
            }

            string cacheKey = $"{id}";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out GeneLiteVM gene))
                {
                    string apiUrl = $"{_apiBaseUrl}/gene/by-id/{id}";
                    gene = await SendRequestAsync<GeneLiteVM>(apiUrl, HttpMethod.Get);

                    if (gene != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, gene, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Nothing found for ID: {id}");
                    }
                }

                return gene;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching ID {id}");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }


    }
}
