
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Commands.NewHit;
using Screen.Application.Features.Commands.UpdateHitCollection;

namespace Screen.API.Controllers.V2
{
    public partial class HitCollectionController : ControllerBase
    {
        [HttpPost("{hitCollectionId}/add-hit", Name = "AddHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> AddHit(Guid hitCollectionId, NewHitCommand command)
        {
            var hitId = Guid.NewGuid();
            command.Id = hitCollectionId;
            command.HitCollectionId = hitCollectionId;

            try
            {
                command.HitId = hitId;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = hitId,
                    Message = "Hit added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddHit: ArgumentNullException {Id}", hitId);
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

        [HttpPost("{hitCollectionId}/update-hit/{hitId}", Name = "UpdateHit")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult> UpdateHit (Guid hitCollectionId, Guid hitId, UpdateHitCollectionCommand command)
        {
            command.Id = hitCollectionId;
            command.HitCollectionId = hitCollectionId;
            command.HitId = hitId;
        
            try
            {
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Screen Run updated successfully",
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

            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the hit";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = hitId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}