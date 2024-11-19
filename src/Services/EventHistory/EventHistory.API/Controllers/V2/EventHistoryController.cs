using System.Net;
using EventHistory.Application.Features.Queries.GetEventHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventHistory.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class EventHistoryController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpGet(Name = "GetEventHistory")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<EventHistoryVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<EventHistoryVM>>> GetEventHistory([FromQuery] GetEventHistoryQuery query)
        {
            var eventHistory = await _mediator.Send(query);
            return Ok(eventHistory);
        }
    }
}