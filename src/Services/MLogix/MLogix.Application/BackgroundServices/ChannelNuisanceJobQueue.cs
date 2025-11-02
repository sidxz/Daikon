using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MLogix.Application.BackgroundServices
{
    public class ChannelNuisanceJobQueue : INuisanceJobQueue
    {
        private readonly Channel<NuisanceJob> _channel;

        public ChannelNuisanceJobQueue()
        {
            // Bounded to prevent unbounded memory growth; tune as needed.
            var options = new BoundedChannelOptions(capacity: 10_000)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<NuisanceJob>(options);
        }

        public ValueTask EnqueueAsync(NuisanceJob job, CancellationToken ct = default)
            => _channel.Writer.WriteAsync(job, ct);

        public async IAsyncEnumerable<NuisanceJob> DequeueAllAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
        {
            while (await _channel.Reader.WaitToReadAsync(ct).ConfigureAwait(false))
            {
                while (_channel.Reader.TryRead(out var job))
                    yield return job;
            }
        }
    }
}