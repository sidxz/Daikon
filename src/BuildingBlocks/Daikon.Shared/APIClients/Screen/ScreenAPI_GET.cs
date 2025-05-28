
using Daikon.Shared.VM.Screen;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


namespace Daikon.Shared.APIClients.Screen
{
    public partial class ScreenAPI
    {
        public async Task<ScreenVM> GetById(Guid id, bool forceRefresh = false)
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
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out ScreenVM vm))
                {
                    string apiUrl = $"{_apiBaseUrl}/screen/{id}";
                    vm = await SendRequestAsync<ScreenVM>(apiUrl, HttpMethod.Get);

                    if (vm != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, vm, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Nothing found for ID: {id}");
                    }
                }

                return vm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching ID {id}");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }



        public async Task<HitCollectionVM> GetHitCollectionById(Guid id, bool forceRefresh = false)
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
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out HitCollectionVM vm))
                {
                    string apiUrl = $"{_apiBaseUrl}/hit-collection/{id}";
                    vm = await SendRequestAsync<HitCollectionVM>(apiUrl, HttpMethod.Get);

                    if (vm != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, vm, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Nothing found for ID: {id}");
                    }
                }

                return vm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching ID {id}");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }

    }
}
