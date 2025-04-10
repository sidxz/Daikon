
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

        public void Start(int totalAggregates, bool isDryRun, string? eventTypeFilter, DateTime? fromDate, DateTime? toDate)
        {
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

        public void UpdateCurrent(Guid aggregateId, int eventCount)
        {
            CurrentAggregate = aggregateId.ToString();
            TotalEvents += eventCount;
        }

        public void MarkAggregateComplete(int eventCount)
        {
            ProcessedAggregates++;
            ProcessedEvents += eventCount;
            CurrentAggregate = null;
        }

        public void Finish()
        {
            IsReplaying = false;
            CancelToken?.Dispose();
            CancelToken = null;
        }

        public void Cancel()
        {
            CancelToken?.Cancel();
        }
    }

}