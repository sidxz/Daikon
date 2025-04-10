/*
 * ReplayStatusTracker
 * -------------------
 * Tracks the progress and status of an event replay operation.
 *
 * Responsibilities:
 * - Maintains counters for total/processed aggregates and events.
 * - Exposes replay configuration (e.g., filters, dry run flag).
 * - Provides cancellation support via CancellationTokenSource.
 * - Helps monitor replay progress for UI or logs.
 */

namespace Daikon.EventManagement.Services
{
    public class ReplayStatusTracker
    {
        public bool IsReplaying { get; private set; }
        public int TotalAggregates { get; private set; }
        public int ProcessedAggregates { get; private set; }
        public int TotalEvents { get; private set; }
        public int ProcessedEvents { get; private set; }
        public string? CurrentAggregate { get; private set; }
        public bool IsDryRun { get; private set; }
        public string? EventTypeFilter { get; private set; }
        public DateTime? FromDate { get; private set; }
        public DateTime? ToDate { get; private set; }
        public CancellationTokenSource? CancelToken { get; private set; }

        /*
         * Initializes the tracker for a new replay session.
         */
        public void Start(int totalAggregates, bool isDryRun, string? eventTypeFilter, DateTime? fromDate, DateTime? toDate)
        {
            /* Dispose previous token if any to avoid leaks */
            CancelToken?.Dispose();

            IsReplaying = true;
            TotalAggregates = totalAggregates;
            ProcessedAggregates = 0;
            TotalEvents = 0;
            ProcessedEvents = 0;
            CurrentAggregate = null;
            IsDryRun = isDryRun;
            EventTypeFilter = eventTypeFilter;
            FromDate = fromDate;
            ToDate = toDate;
            CancelToken = new CancellationTokenSource();
        }

        /*
         * Updates current progress with a new aggregate being processed.
         */
        public void UpdateCurrent(Guid aggregateId, int eventCount)
        {
            CurrentAggregate = aggregateId.ToString();
            TotalEvents += eventCount;
        }

        /*
         * Marks the current aggregate as fully processed.
         */
        public void MarkAggregateComplete(int eventCount)
        {
            ProcessedAggregates++;
            ProcessedEvents += eventCount;
            CurrentAggregate = null;
        }

        /*
         * Finalizes the tracking session and releases resources.
         */
        public void Finish()
        {
            IsReplaying = false;
            CancelToken?.Dispose();
            CancelToken = null;
        }

        /*
         * Signals cancellation to any listeners observing the token.
         */
        public void Cancel()
        {
            if (CancelToken?.IsCancellationRequested == false)
            {
                CancelToken.Cancel();
            }
        }
    }
}
