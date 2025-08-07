
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

using Daikon.Shared.APIClients.Common;

namespace Daikon.Shared.APIClients.Horizon
{
    public partial class HorizonAPI : APIRequests, IHorizonAPI
    {
        private readonly ILogger<HorizonAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public HorizonAPI(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ILogger<HorizonAPI> logger, IConfiguration configuration)
         : base(httpClient, httpContextAccessor, logger)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["HorizonAPI:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
        }
    }
}
