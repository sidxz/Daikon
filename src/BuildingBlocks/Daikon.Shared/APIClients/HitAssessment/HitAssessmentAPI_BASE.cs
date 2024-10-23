using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Net.Http.Headers;
using Daikon.Shared.APIClients.Common;

namespace Daikon.Shared.APIClients.HitAssessment
{
    public partial class HitAssessmentAPI : APIRequests, IHitAssessmentAPI
    {
        private readonly ILogger<HitAssessmentAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public HitAssessmentAPI(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, ILogger<HitAssessmentAPI> logger, IConfiguration configuration)
         : base(httpClient, httpContextAccessor, logger)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["HitAssessmentAPI:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
        }
    }
}
