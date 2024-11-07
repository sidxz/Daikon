
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

using Daikon.Shared.APIClients.Common;

namespace Daikon.Shared.APIClients.Gene
{
    public partial class GeneAPI : APIRequests, IGeneAPI
    {
        private readonly ILogger<GeneAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public GeneAPI(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ILogger<GeneAPI> logger, IConfiguration configuration)
         : base(httpClient, httpContextAccessor, logger)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["GeneAPI:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
        }
    }
}
