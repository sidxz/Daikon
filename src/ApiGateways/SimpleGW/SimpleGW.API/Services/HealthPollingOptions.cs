using System;
using System.Collections.Generic;

namespace SimpleGW.API.Services
{
    public sealed class HealthPollingOptions
    {
        public int IntervalSeconds { get; init; } = 30;
        public int TimeoutSeconds { get; init; } = 10;
        public Dictionary<string, HealthPollingServiceOverride> ServiceOverrides { get; init; } =
            new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, HealthPollingTarget> ExternalServices { get; init; } =
            new(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class HealthPollingServiceOverride
    {
        public int? TimeoutSeconds { get; init; }
        public string? HealthPath { get; init; }
    }

    public sealed class HealthPollingTarget
    {
        public string BaseUrl { get; init; } = string.Empty;
        public string? HealthPath { get; init; }
        public int? TimeoutSeconds { get; init; }
    }
}
