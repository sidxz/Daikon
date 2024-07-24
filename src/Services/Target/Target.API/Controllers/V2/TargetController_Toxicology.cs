using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Target.Application.Features.Commands.AddOrUpdateToxicology;
using Target.Application.Features.Commands.AddToxicology;
using Target.Application.Features.Commands.DeleteToxicology;
using Target.Application.Features.Commands.UpdateToxicology;

namespace Target.API.Controllers.V2
{
    public partial class TargetController : ControllerBase
    {
        [HttpPost("{id}/toxicology/add-update", Name = "AddOrToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddOrUpdateToxicology(Guid id, AddOrUpdateToxicologyCommand command)
        {
            try
            {
                command.Id = id;
                command.TargetId = id;

                await _mediator.Send(command);

                return Ok(new BaseResponse
                {
                    Message = "Toxicology added or updated successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddOrUpdateToxicology: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding or updating the toxicology";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPost("{id}/toxicology", Name = "AddToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddToxicology(Guid id, AddToxicologyCommand command)
        {
            var toxicologyId = Guid.NewGuid();
            try
            {
                command.ToxicologyId = toxicologyId;
                command.Id = id;
                command.TargetId = id;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = toxicologyId,
                    Message = "Toxicology added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddToxicology: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the toxicology";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = toxicologyId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }


        [HttpPut("{id}/toxicology/{toxicologyId}", Name = "UpdateToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateToxicology(Guid id, Guid toxicologyId, UpdateToxicologyCommand command)
        {
            try
            {
                command.Id = id;
                command.ToxicologyId = toxicologyId;
                command.TargetId = id;

                await _mediator.Send(command);

                return Ok(new BaseResponse
                {
                    Message = "Toxicology updated successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("UpdateToxicology: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the toxicology";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpDelete("{id}/toxicology/{toxicologyId}", Name = "DeleteToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteToxicology(Guid id, Guid toxicologyId)
        {
            try
            {
                var command = new DeleteToxicologyCommand
                {
                    Id = id,
                    ToxicologyId = toxicologyId,
                    TargetId = id
                };

                await _mediator.Send(command);

                return Ok(new BaseResponse
                {
                    Message = "Toxicology deleted successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("DeleteToxicology: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the toxicology";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}