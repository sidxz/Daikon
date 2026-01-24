using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleGW.API.Services
{
    public sealed class MicroserviceHealthPoller : BackgroundService
    {
        private const string DefaultHealthPath = "/health";
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMicroserviceHealthStore _healthStore;
        private readonly ILogger<MicroserviceHealthPoller> _logger;

        public MicroserviceHealthPoller(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMicroserviceHealthStore healthStore,
            ILogger<MicroserviceHealthPoller> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _healthStore = healthStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var options = _configuration.GetSection("HealthPolling").Get<HealthPollingOptions>()
                              ?? new HealthPollingOptions();
                var interval = TimeSpan.FromSeconds(Math.Max(1, options.IntervalSeconds));
                var timeoutSeconds = Math.Max(1, options.TimeoutSeconds);

                var endPointRouting = _configuration.GetSection("EndPointRouting").Get<Dictionary<string, string>>()
                                      ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var externalServices = options.ExternalServices ??
                                       new Dictionary<string, HealthPollingTarget>(StringComparer.OrdinalIgnoreCase);

                if (endPointRouting.Count == 0 && externalServices.Count == 0)
                {
                    _logger.LogWarning("EndPointRouting and HealthPolling:ExternalServices are empty; skipping health polling.");
                }
                else
                {
                    var client = _httpClientFactory.CreateClient("SimpleGWClient");

                    var targets = new List<PollTarget>();

                    targets.AddRange(endPointRouting.Select(route =>
                    {
                        var serviceName = route.Key;
                        var serviceOverride = options.ServiceOverrides.TryGetValue(serviceName, out var overrideOptions)
                            ? overrideOptions
                            : null;
                        var healthPath = serviceOverride?.HealthPath ?? DefaultHealthPath;
                        var serviceTimeoutSeconds = Math.Max(1, serviceOverride?.TimeoutSeconds ?? timeoutSeconds);
                        return new PollTarget(serviceName, route.Value, healthPath, serviceTimeoutSeconds);
                    }));

                    targets.AddRange(externalServices.Select(external =>
                    {
                        var serviceName = external.Key;
                        var target = external.Value;
                        var healthPath = target.HealthPath ?? DefaultHealthPath;
                        var serviceTimeoutSeconds = Math.Max(1, target.TimeoutSeconds ?? timeoutSeconds);
                        return new PollTarget(serviceName, target.BaseUrl, healthPath, serviceTimeoutSeconds);
                    }));

                    foreach (var target in targets)
                    {
                        if (string.IsNullOrWhiteSpace(target.BaseUrl))
                        {
                            _logger.LogWarning("Health polling base URL is empty for {ServiceName}; skipping.", target.ServiceName);
                            continue;
                        }

                        var healthUrl = $"{target.BaseUrl.TrimEnd('/')}/{target.HealthPath.TrimStart('/')}";
                        var checkedAt = DateTimeOffset.UtcNow;

                        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        timeoutCts.CancelAfter(TimeSpan.FromSeconds(target.TimeoutSeconds));

                        try
                        {
                            var response = await client.GetAsync(healthUrl, timeoutCts.Token);
                            var body = await response.Content.ReadAsStringAsync(timeoutCts.Token);
                            var bodyPayload = TryParseJson(body, out var parsedBody)
                                ? (object)parsedBody
                                : body;

                            _healthStore.Update(
                                target.ServiceName,
                                new MicroserviceHealthStatus((int)response.StatusCode, bodyPayload, checkedAt, null));
                        }
                        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogWarning(ex, "Health check failed for {ServiceName} at {HealthUrl}", target.ServiceName, healthUrl);
                            _healthStore.Update(
                                target.ServiceName,
                                new MicroserviceHealthStatus(null, null, checkedAt, ex.Message));
                        }
                    }
                }

                try
                {
                    await Task.Delay(interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private static bool TryParseJson(string body, out JsonElement parsed)
        {
            parsed = default;
            if (string.IsNullOrWhiteSpace(body))
            {
                return false;
            }

            try
            {
                using var document = JsonDocument.Parse(body);
                parsed = document.RootElement.Clone();
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private sealed record PollTarget(string ServiceName, string BaseUrl, string HealthPath, int TimeoutSeconds);
    }
}
