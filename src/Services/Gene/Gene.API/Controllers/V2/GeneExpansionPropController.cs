using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.AddExpansionProp;
using Gene.Application.Features.Command.DeleteExpansionProp;
using Gene.Application.Features.Command.UpdateExpansionProp;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    public partial class GeneController : ControllerBase
    {
        [HttpPost("{id}/expansion-prop", Name = "AddExpansionProp")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddExpansionProp(Guid id, AddExpansionPropCommand command)
        {
            var expansionPropId = Guid.NewGuid();
            try
            {
                command.Id = id;
                command.ExpansionPropId = expansionPropId;
                
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = expansionPropId,
                    Message = "ExpansionProp added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddExpansionProp: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the expansionprop";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = expansionPropId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPut("{id}/expansion-prop/{expansionPropId}", Name = "UpdateExpansionProp")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateExpansionProp(Guid id, Guid expansionPropId, UpdateExpansionPropCommand command)
        {
            try
            {
                command.Id = id;
                command.ExpansionPropId = expansionPropId;

                await _mediator.Send(command);

                return Ok(new BaseResponse
                {
                    Message = "ExpansionProp updated successfully"
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "UpdateExpansionProp: Resource not found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the expansionprop";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpDelete("{id}/expansion-prop/{expansionPropId}", Name = "DeleteExpansionProp")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteExpansionProp(Guid id, Guid expansionPropId)
        {
            try
            {
                var command = new DeleteExpansionPropCommand
                {
                    Id = id,
                    ExpansionPropId = expansionPropId
                };

                await _mediator.Send(command);

                return Ok(new BaseResponse
                {
                    Message = "ExpansionProp deleted successfully"
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "DeleteExpansionProp: Resource not found");
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the expansionprop";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
        
    }
}