using Daikon.EventManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Daikon.EventManagement.Controllers
{
    [ApiController]
    [Route("api/replay")]
    public class ReplayController : ControllerBase
    {
        private readonly EventReplayService _replayService;
        private readonly ReplayStatusTracker _tracker;

        public ReplayController(EventReplayService replayService, ReplayStatusTracker tracker)
        {
            _replayService = replayService;
            _tracker = tracker;
        }

        [HttpPost("start")]
        public IActionResult StartReplay(
            [FromQuery] string? topic = null,
            [FromQuery] bool dryRun = false,
            [FromQuery] string? eventType = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            if (_tracker.IsReplaying)
                return Conflict("Replay already in progress.");

            _ = Task.Run(() => _replayService.ReplayAllEventsAsync(topic, dryRun, eventType, fromDate, toDate));
            return Accepted("Replay started.");
        }

        [HttpGet("status")]
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

        [HttpPost("cancel")]
        public IActionResult Cancel()
        {
            if (!_tracker.IsReplaying)
                return BadRequest("No active replay to cancel.");

            _tracker.Cancel();
            return Ok("Replay cancel requested.");
        }


        
    }
}