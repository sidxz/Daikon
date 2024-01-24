
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Commands.DeleteScreenRun;
using Screen.Application.Features.Commands.NewScreenRun;
using Screen.Application.Features.Commands.UpdateScreenRun;

namespace Screen.API.Controllers.V2
{
    public partial class ScreenController : ControllerBase
    {
        [HttpPost("{screenId}/add-screen-run", Name = "AddScreenRun")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddScreenRun(Guid screenId, NewScreenRunCommand command)
        {
            command.Id = screenId;    
            command.ScreenRunId = Guid.NewGuid();
            try
            {
                
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = command.ScreenRunId,
                    Message = "Screen Run added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("screenRunId: Requested Resource Already Exists {Id}", ex.Message);
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
                    Id = command.ScreenRunId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }


        [HttpPost("{screenId}/update-screen-run/{screenRunId}", Name = "UpdateScreenRun")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateScreenRun(Guid screenId, Guid screenRunId, UpdateScreenRunCommand command)
        {
            command.Id = screenId;
            command.ScreenRunId = screenRunId;

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
                _logger.LogInformation("UpdateScreenRun: ArgumentNullException {Id}", screenRunId);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("screenRunId: Requested Resource Already Exists {Id}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the screen run";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpDelete("{screenId}/delete-screen-run/{screenRunId}", Name = "DeleteScreenRun")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteScreenRun(Guid screenId, Guid screenRunId)
        {
            try
            {
                await _mediator.Send(new DeleteScreenRunCommand { Id = screenId, ScreenRunId = screenRunId });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Screen Run deleted successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("DeleteScreenRun: ArgumentNullException {Id}", screenRunId);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the screen run";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}