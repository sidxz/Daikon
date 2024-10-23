
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Daikon.Shared.VM.HitAssessment;

namespace Daikon.Shared.APIClients.HitAssessment
{
    public partial class HitAssessmentAPI
    {
        public async Task<HitAssessmentVM> GetById(Guid id, bool forceRefresh = false)
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
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out HitAssessmentVM ha))
                {
                    string apiUrl = $"{_apiBaseUrl}/{id}";
                    ha = await SendRequestAsync<HitAssessmentVM>(apiUrl, HttpMethod.Get);

                    if (ha != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, ha, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Nothing found for ID: {id}");
                    }
                }

                return ha;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching ID {id}");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }
        public async Task<List<HitAssessmentVM>> GetList(bool forceRefresh = false)
        {
            string cacheKey = "List_";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out List<HitAssessmentVM> hAs))
                {
                    string apiUrl = $"{_apiBaseUrl}/";
                    hAs = await SendRequestAsync<List<HitAssessmentVM>>(apiUrl, HttpMethod.Get);

                    if (hAs != null && hAs.Count > 0)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, hAs, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning("No data returned from the API.");
                    }
                }

                return hAs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching list.");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }
    }
}
