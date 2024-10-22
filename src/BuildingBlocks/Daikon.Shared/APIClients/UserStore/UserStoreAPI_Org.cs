using Daikon.Shared.VM.UserStore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daikon.Shared.APIClients.UserStore
{
    public partial class UserStoreAPI
    {
        /// <summary>
        /// Retrieves an organization by its ID. Data is fetched from cache unless forceRefresh is true or the cache is invalid.
        /// </summary>
        public async Task<AppOrgVM> GetOrgById(Guid id, bool forceRefresh = false)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid organization ID provided.");
                throw new ArgumentException("Invalid organization ID.", nameof(id));
            }

            string cacheKey = $"Org_{id}";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out AppOrgVM org))
                {
                    string apiUrl = $"{_apiBaseUrl}/org/{id}";
                    org = await SendRequestAsync<AppOrgVM>(apiUrl, HttpMethod.Get);

                    if (org != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, org, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"No organization found for ID: {id}");
                    }
                }

                return org;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching organization with ID {id}");
                throw new ApplicationException($"Unable to retrieve organization information. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a list of organizations. Data is fetched from cache unless forceRefresh is true or the cache is invalid.
        /// </summary>
        public async Task<List<AppOrgVM>> GetOrgs(bool forceRefresh = false)
        {
            string cacheKey = "Orgs";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out List<AppOrgVM> orgs))
                {
                    string apiUrl = $"{_apiBaseUrl}/org";
                    orgs = await SendRequestAsync<List<AppOrgVM>>(apiUrl, HttpMethod.Get);

                    if (orgs != null && orgs.Count > 0)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, orgs, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning("No organizations data returned from the API.");
                    }
                }

                return orgs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching list of organizations.");
                throw new ApplicationException("Unable to retrieve organizations information. Please try again later.");
            }
        }
    }
}
