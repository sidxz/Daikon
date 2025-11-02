using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Daikon.Shared.APIClients.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.CageFusion;
using MLogix.Application.DTOs.CageFusion;

namespace MLogix.Infrastructure.CageFusion
{
    public class NuisanceAPI : APIRequests, INuisanceAPI
    {

        private readonly ILogger<NuisanceAPI> _logger;
        private readonly string _apiBaseUrl;
        public NuisanceAPI(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<NuisanceAPI> logger, IConfiguration configuration) : base(httpClient, httpContextAccessor, logger)
        {
            _logger = logger;
            _apiBaseUrl = configuration["CageFusion:NuisanceAPIURL"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
        }

        public async Task<NuisanceResponseDTO> GetNuisancePredictionsAsync(NuisanceRequestDTO request)
        {
            try
            {
                string apiUrl = $"{_apiBaseUrl}/cage-fusion-api/predict";
                return await SendRequestAsync<NuisanceResponseDTO>(apiUrl, System.Net.Http.HttpMethod.Post, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting nuisance predictions.");
                throw;
            }
        }
    }
}