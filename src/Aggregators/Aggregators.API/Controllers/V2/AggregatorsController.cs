
using System.Net;
using Aggregators.Application.Disclosure.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Aggregators.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class AggregatorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AggregatorsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("ping", Name = "ping")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Ping()
        {
            return await Task.FromResult(Ok("Pong"));
        }

        [HttpGet("disclosure/generate-dashboard", Name = "GenerateDashboard")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GenerateDashboard([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var query = new GenerateDashQuery { StartDate = startDate, EndDate = endDate };
            var dashboard = await _mediator.Send(query);
            return Ok(dashboard);
        }

        

    }
}