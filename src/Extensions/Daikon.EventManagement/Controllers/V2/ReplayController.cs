using Daikon.EventManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Daikon.EventManagement.Controllers.V2
{
    /*
     * ReplayController
     * ----------------
     * Exposes HTTP endpoints to trigger, monitor, and cancel event replay operations.
     * Useful for operational tooling, dashboard control, or automation via API.
     */
    [ApiController]
    [ApiVersion("2.0")]

    [Route("api/replay")]
    public class ReplayController : ControllerBase
    {
        private readonly EventReplayService _replayService;
        private readonly ReplayStatusTracker _tracker;
        private static readonly object _lock = new();

        public ReplayController(EventReplayService replayService, ReplayStatusTracker tracker)
        {
            _replayService = replayService;
            _tracker = tracker;
        }

        /*
         * Starts a background event replay operation with optional filters.
         */
        [HttpPost("start")]
        [MapToApiVersion("2.0")]
        public IActionResult StartReplay(
            [FromQuery] string? topic = null,
            [FromQuery] bool dryRun = false,
            [FromQuery] string? eventType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            lock (_lock)
            {
                if (_tracker.IsReplaying)
                    return Conflict("Replay already in progress.");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _replayService.ReplayAllEventsAsync(topic, dryRun, eventType, fromDate, toDate);
                    }
                    catch (Exception ex)
                    {
                        // Optionally use a logger or an alerting system here
                        Console.Error.WriteLine($"[ReplayController] Fatal error during replay: {ex}");
                    }
                });

                return Accepted("Replay started.");
            }
        }

        /*
         * Returns current status of the replay tracker.
         */
        [HttpGet("status")]
        [MapToApiVersion("2.0")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                _tracker.IsReplaying,
                _tracker.IsDryRun,
                _tracker.TotalAggregates,
                _tracker.ProcessedAggregates,
                _tracker.TotalEvents,
                _tracker.ProcessedEvents,
                _tracker.CurrentAggregate,
                _tracker.EventTypeFilter,
                _tracker.FromDate,
                _tracker.ToDate
            });
        }

        /*
         * Cancels the currently running replay (if any).
         */
        [HttpPost("cancel")]
        [MapToApiVersion("2.0")]
        public IActionResult Cancel()
        {
            if (!_tracker.IsReplaying)
                return BadRequest("No active replay to cancel.");

            _tracker.Cancel();
            return Ok("Replay cancel requested.");
        }
    }
}
