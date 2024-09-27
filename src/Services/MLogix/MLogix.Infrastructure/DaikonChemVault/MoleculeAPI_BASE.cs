using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Features.Queries.Filters;

namespace MLogix.Infrastructure.DaikonChemVault
{
    public partial class MoleculeAPI : IMoleculeAPI
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MoleculeAPI> _logger;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        public MoleculeAPI(ILogger<MoleculeAPI> logger, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiBaseUrl = configuration["DaikonChemVault:Url"] ?? throw new ArgumentNullException(nameof(_apiBaseUrl));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        private async Task<T> SendRequestAsync<T>(string apiUrl, HttpMethod method, IDictionary<string, string> headers)
        {
            try
            {
                var request = new HttpRequestMessage(method, apiUrl);
                request.AddHeaders(headers);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(result, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("API request failed. URL: {ApiUrl}, Status Code: {StatusCode}", apiUrl, response.StatusCode);
                    return default;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error for URL: {ApiUrl}", apiUrl);
                throw new HttpRequestException("Error while making API request", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing JSON response from URL: {ApiUrl}", apiUrl);
                throw new JsonException("Error parsing response data", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during API request to URL: {ApiUrl}", apiUrl);
                throw new ApplicationException("Unexpected error", ex);
            }
        }

        private string BuildQueryString(IDictionary<string, object> queryParams)
        {
            return string.Join("&", queryParams
                .Where(kvp => kvp.Value != null)
                .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value.ToString())}"));
        }


        private void AddConditionFilters<T>(T query, Dictionary<string, object> queryParams) where T : BaseQueryWithConditionFilters
        {
            var filters = typeof(T)
                .GetProperties()
                .Where(p => p.GetValue(query) != null);

            foreach (var filter in filters)
            {
                object value = filter.GetValue(query);

                // Check if the property has a JsonPropertyName attribute
                var jsonPropertyName = filter.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                string name = jsonPropertyName ?? filter.Name;

                // Add the filter to query params
                queryParams.Add(name, value);
            }
        }

    }
}