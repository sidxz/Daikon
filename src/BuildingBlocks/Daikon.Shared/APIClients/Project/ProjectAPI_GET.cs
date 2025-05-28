
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Daikon.Shared.VM.Project;

namespace Daikon.Shared.APIClients.Project
{
    public partial class ProjectAPI
    {
        public async Task<ProjectVM> GetById(Guid id, bool forceRefresh = false)
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
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out ProjectVM project))
                {
                    string apiUrl = $"{_apiBaseUrl}/project/by-id/{id}";
                    project = await SendRequestAsync<ProjectVM>(apiUrl, HttpMethod.Get);

                    if (project != null)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, project, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"Nothing found for ID: {id}");
                    }
                }

                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching ID {id}");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }
        public async Task<List<ProjectListVM>> GetList(bool forceRefresh = false)
        {
            string cacheKey = "List_";

            try
            {
                // Check if force refresh is requested or cache is invalid
                if (forceRefresh || !_cache.TryGetValue(cacheKey, out List<ProjectListVM> projects))
                {
                    string apiUrl = $"{_apiBaseUrl}/project";
                    projects = await SendRequestAsync<List<ProjectListVM>>(apiUrl, HttpMethod.Get);

                    if (projects != null && projects.Count > 0)
                    {
                        // Store the result in cache
                        _cache.Set(cacheKey, projects, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheDuration
                        });
                    }
                    else
                    {
                        _logger.LogWarning("No data returned from the API.");
                    }
                }

                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching list.");
                throw new ApplicationException("Unable to retrieve information. Please try again later.");
            }
        }
    }
}
