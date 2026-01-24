using System;

namespace SimpleGW.API.Services
{
    public sealed record MicroserviceHealthStatus(
        int? StatusCode,
        object? Body,
        DateTimeOffset LastCheckedUtc,
        string? Error);
}
