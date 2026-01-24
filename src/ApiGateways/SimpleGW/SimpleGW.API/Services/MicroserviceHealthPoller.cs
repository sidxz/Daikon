using System;
using System.Collections.Generic;
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

                if (endPointRouting.Count == 0)
                {
                    _logger.LogWarning("EndPointRouting configuration is empty; skipping health polling.");
                }
                else
                {
                    var client = _httpClientFactory.CreateClient("SimpleGWClient");
                    foreach (var route in endPointRouting)
                    {
                        var serviceName = route.Key;
                        var baseUrl = route.Value;
                        var serviceOverride = options.ServiceOverrides.TryGetValue(serviceName, out var overrideOptions)
                            ? overrideOptions
                            : null;
                        var healthPath = serviceOverride?.HealthPath ?? DefaultHealthPath;
                        var serviceTimeoutSeconds = Math.Max(1, serviceOverride?.TimeoutSeconds ?? timeoutSeconds);
                        var healthUrl = $"{baseUrl.TrimEnd('/')}/{healthPath.TrimStart('/')}";
                        var checkedAt = DateTimeOffset.UtcNow;

                        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        timeoutCts.CancelAfter(TimeSpan.FromSeconds(serviceTimeoutSeconds));

                        try
                        {
                            var response = await client.GetAsync(healthUrl, timeoutCts.Token);
                            var body = await response.Content.ReadAsStringAsync(timeoutCts.Token);

                            _healthStore.Update(
                                serviceName,
                                new MicroserviceHealthStatus((int)response.StatusCode, body, checkedAt, null));
                        }
                        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogWarning(ex, "Health check failed for {ServiceName} at {HealthUrl}", serviceName, healthUrl);
                            _healthStore.Update(
                                serviceName,
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
    }
}
