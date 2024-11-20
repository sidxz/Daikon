
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

using Daikon.Shared.APIClients.Common;

namespace Daikon.Shared.APIClients.Project
{
    public partial class ProjectAPI : APIRequests, IProjectAPI
    {
        private readonly ILogger<ProjectAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public ProjectAPI(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ILogger<ProjectAPI> logger, IConfiguration configuration)
         : base(httpClient, httpContextAccessor, logger)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["ProjectAPI:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
        }
    }
}
