
using System.Net;
using System.Text.Json;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Target.Application.Features.Commands.ApproveTarget;
using Target.Application.Features.Commands.SubmitTPQ;
using Target.Application.Features.Commands.UpdateTPQ;
using Target.Application.Features.Queries.GetTPQ;
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
            try
            {
                var unverifiedResponses = await _mediator.Send(new ListTPQRespUnverifiedQuery { WithMeta = WithMeta });
                return Ok(unverifiedResponses);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("ListTPQRespUnverified: Requested Resource Not Found");
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the ListTPQRespUnverified";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Get method for GetTPQ
        [HttpGet("tpq/{id}", Name = "GetTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(PQResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PQResponse>> GetTPQ(Guid id, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var tpq = await _mediator.Send(new GetTPQQuery { Id = id, WithMeta = WithMeta });
                return Ok(tpq);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetTPQ: Requested Resource Not Found");
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the TPQ";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Post method for SubmitTPQ
        [HttpPost("tpq", Name = "SubmitTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> SubmitTPQ([FromBody] SubmitTPQCommand submitTPQCommand)
        {
            _logger.LogInformation($"Received SubmitTPQCommand: {JsonSerializer.Serialize(submitTPQCommand)}");

            var id = Guid.NewGuid();
            try
            {
                submitTPQCommand.Id = id;
                var response = await _mediator.Send(submitTPQCommand);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("SubmitTPQ: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("SubmitTPQ: Requested Resource Already Exists {Name}", ex.Message);
                return Conflict(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the TPQ";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Put method for UpdateTPQ
        [HttpPut("tpq/{id}", Name = "UpdateTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> UpdateTPQ(Guid id, [FromBody] UpdateTPQCommand updateTPQCommand)
        {
            try
            {
                updateTPQCommand.Id = id;
                var response = await _mediator.Send(updateTPQCommand);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateTPQ: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the TPQ";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Approve
        [HttpPost("tpq/{tpqId}/approve", Name = "ApproveTPQ")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BaseResponse>> ApproveTPQ(Guid tpqId, [FromBody] ApproveTargetCommand approveTargetCommand)
        {
            var targetId = Guid.NewGuid();
            try
            {
                approveTargetCommand.TPQId = tpqId;
                approveTargetCommand.Id = targetId;
                var response = await _mediator.Send(approveTargetCommand);
                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = targetId,
                    Message = "Target added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("ApproveTPQ: ArgumentNullException {TPQId}", tpqId);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while approving the TPQ";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


    }
}