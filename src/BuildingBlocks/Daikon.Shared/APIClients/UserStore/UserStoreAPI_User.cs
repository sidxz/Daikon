using Daikon.Shared.VM.UserStore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Daikon.Shared.APIClients.UserStore
{
    public partial class UserStoreAPI
    {
        /// <summary>
        /// Retrieves a user by their ID. Data is fetched from cache unless forceRefresh is true or the cache is invalid.
        /// </summary>
        public async Task<AppUserVM> GetUserById(Guid id, bool forceRefresh = false)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid user ID provided.");
                throw new ArgumentException("Invalid user ID.", nameof(id));
            }

            string cacheKey = $"User_{id}";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out AppUserVM user))
                {
                    string apiUrl = $"{_apiBaseUrl}/user/{id}";
                    user = await SendRequestAsync<AppUserVM>(apiUrl, HttpMethod.Get);

                    if (user != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, user, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"No user found for ID: {id}");
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user with ID {id}");
                throw new ApplicationException("Unable to retrieve user information. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a list of users. Data is fetched from cache unless forceRefresh is true or the cache is invalid.
        /// </summary>
        public async Task<List<AppUserVM>> GetUsers(bool forceRefresh = false)
        {
            string cacheKey = "Users";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out List<AppUserVM> users))
                {
                    string apiUrl = $"{_apiBaseUrl}/user";
                    users = await SendRequestAsync<List<AppUserVM>>(apiUrl, HttpMethod.Get);

                    if (users != null && users.Count > 0)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, users, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning("No users data returned from the API.");
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching list of users.");
                throw new ApplicationException("Unable to retrieve users information. Please try again later.");
            }
        }
    }
}
