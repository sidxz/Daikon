
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Commands.DeleteHit;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Commands.UpdateHit;
using Screen.Application.Features.Queries.GetHit.ById;
using Screen.Application.Features.Views.GetHitProperties;
using Screen.Domain.Entities;

namespace Screen.API.Controllers.V2
{
    public partial class HitCollectionController : ControllerBase
    {
        [HttpPost("{id}/hit", Name = "AddHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> AddHit(Guid id, NewHitCommand command)
        {
            var hitId = Guid.NewGuid();
            command.Id = id;
            command.HitId = hitId;
            try
            {
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = hitId,
                    Message = "Hit added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddHit: ArgumentNullException {Id}", command.HitId);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("hitId: Requested Resource Already Exists {Id}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the screen run";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = hitId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPut("{id}/hit/{hitId}", Name = "UpdateHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> UpdateHit(Guid id, Guid hitId, UpdateHitCommand command)
        {
            command.Id = id;
            command.HitId = hitId;

            
            try
            {
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Hit updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateHit: ArgumentNullException {Id}", hitId);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("hitId: Requested Resource Already Exists {Id}", ex.Message);
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
            catch (AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the hit";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = hitId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpDelete("{id}/hit/{hitId}", Name = "DeleteHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteHit(Guid id, Guid hitId)
        {
            try
            {
                await _mediator.Send(new DeleteHitCommand { Id = id, HitId = hitId });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Hit deleted successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("DeleteHit: ArgumentNullException {Id}", hitId);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the hit";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("hit/{hitId}", Name = "GetHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Hit>> GetHit(Guid hitId)
        {
            try
            {
                var hit = await _mediator.Send(new GetHitByIdCommand { Id = hitId });

                return StatusCode(StatusCodes.Status200OK, hit);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("GetHit: ArgumentNullException {Id}", hitId);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching hit";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("view/hit/{hitId}", Name = "GetHitView")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<HitPropertiesVM>> GetHitView(Guid hitId)
        {
            try
            {
                var hitProperties = await _mediator.Send(new GetHitPropertiesQuery { Id = hitId });

                return StatusCode(StatusCodes.Status200OK, hitProperties);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("GetHitView: ArgumentNullException {Id}", hitId);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching hit properties";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}