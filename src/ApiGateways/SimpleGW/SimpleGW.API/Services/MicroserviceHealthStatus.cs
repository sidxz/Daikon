using System;

namespace SimpleGW.API.Services
{
    public sealed record MicroserviceHealthStatus(
        int? StatusCode,
        string? Body,
        DateTimeOffset LastCheckedUtc,
        string? Error);
}
