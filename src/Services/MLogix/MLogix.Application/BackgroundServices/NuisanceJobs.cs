using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Application.Features.Commands.PredictNuisance;

namespace MLogix.Application.BackgroundServices
{
    public sealed record NuisanceJob(
    PredictNuisanceCommand Command,
    string? CorrelationId = null
);

    public interface INuisanceJobQueue
    {
        ValueTask EnqueueAsync(NuisanceJob job, CancellationToken ct = default);
        IAsyncEnumerable<NuisanceJob> DequeueAllAsync(CancellationToken ct);
    }
}