
using System.Net;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Target.Application.Features.Commands.ApproveTarget;
using Target.Application.Features.Commands.RejectTarget;
using Target.Application.Features.Commands.SubmitTPQ;
using Target.Application.Features.Commands.UpdateTPQ;
using Target.Application.Features.Queries.ListTPQRespUnverified;
using Target.Domain.Entities;

namespace Target.API.Controllers.V2
{

    public partial class TargetController : ControllerBase
    {

        [HttpGet("tpq/unverified", Name = "ListTPQRespUnverified")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<PQResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<PQResponse>>> ListTPQRespUnverified([FromQuery] bool WithMeta = false)
        {
            var unverifiedResponses = await _mediator.Send(new ListTPQRespUnverifiedQuery { WithMeta = WithMeta });
            return Ok(unverifiedResponses);

        }

        // Get method for GetTPQ
        [HttpGet("tpq/{id}", Name = "GetTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(PQResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PQResponse>> GetTPQ(Guid id, [FromQuery] bool WithMeta = false)
        {
            var tpq = await _mediator.Send(new Application.Features.Queries.GetTPQ.ById.GetTPQQuery { Id = id, WithMeta = WithMeta });
            return Ok(tpq);
        }

        // Get method for GetTPQ by TargetId
        [HttpGet("tpq/by-target-id/{targetId}", Name = "GetTPQByTargetId")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(PQResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PQResponse>> GetTPQByTargetId(Guid targetId, [FromQuery] bool WithMeta = false)
        {
            var tpq = await _mediator.Send(new Application.Features.Queries.GetTPQ.ByTargetId.GetTPQQuery { TargetId = targetId, WithMeta = WithMeta });
            return Ok(tpq);

        }

        // Post method for SubmitTPQ
        [HttpPost("tpq", Name = "SubmitTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> SubmitTPQ([FromBody] SubmitTPQCommand submitTPQCommand)
        {

            var id = Guid.NewGuid();
            submitTPQCommand.Id = id;
            var response = await _mediator.Send(submitTPQCommand);
            return Ok(response);
        }

        // Put method for UpdateTPQ
        [HttpPut("tpq/{id}", Name = "UpdateTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> UpdateTPQ(Guid id, [FromBody] UpdateTPQCommand updateTPQCommand)
        {
            updateTPQCommand.Id = id;
            var response = await _mediator.Send(updateTPQCommand);
            return Ok(response);

        }

        // Approve
        [HttpPost("tpq/{tpqId}/approve", Name = "ApproveTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> ApproveTPQ(Guid tpqId, [FromBody] ApproveTargetCommand approveTargetCommand)
        {
            var targetId = Guid.NewGuid();
            approveTargetCommand.TPQId = tpqId;
            approveTargetCommand.Id = targetId;
            var response = await _mediator.Send(approveTargetCommand);
            return StatusCode(StatusCodes.Status201Created, new AddResponse
            {
                Id = targetId,
                Message = "TPQ approved successfully.",
            });

        }

        // Reject & Delete
        [HttpDelete("tpq/{tpqId}", Name = "RejectTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> RejectTPQ(Guid tpqId)
        {
            var rejectTargetCommand = new RejectTargetCommand { Id = tpqId };
            var response = await _mediator.Send(rejectTargetCommand);
            return StatusCode(StatusCodes.Status200OK, new AddResponse
            {
                Id = tpqId,
                Message = "TPQ rejected successfully.",
            });
        }
    }
}