
using Horizon.Application.Features.Queries.FindRelatedTarget;
using Horizon.Application.Features.Queries.FIndTargetRelations;
using Horizon.Application.Features.Queries.GenerateHorizon;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class Horizon(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> Get(Guid id, [FromQuery] bool withMeta = false)
        {
            var horizon = await _mediator.Send(new GenerateHorizonQuery { Id = id, WithMeta = withMeta });
            return Ok(horizon);
        }

        [HttpGet("find-target/{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> FindRelatedTarget(Guid id)
        {
            var relatedTarget = await _mediator.Send(new FindRelatedTargetQuery { Id = id });
            return Ok(relatedTarget);
        }

        [HttpGet("list-target-relations")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> ListTargetRelations()
        {
            var targetRelations = await _mediator.Send(new ListAllTargetRelationsQuery { });
            return Ok(targetRelations);
        }
    }
}