
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteEssentiality;
using Gene.Application.Features.Command.NewEssentiality;
using Gene.Application.Features.Command.UpdateEssentiality;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {
        
        [HttpPost("{id}/essentiality", Name = "AddEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddEssentiality(Guid id, NewEssentialityCommand command)
        {
            var essentialityId = Guid.NewGuid();
            try
            {
                command.EssentialityId = essentialityId;
                command.Id = id;
                command.GeneId = id;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = essentialityId,
                    Message = "Essentiality added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddEssentiality: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the essentiality";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = essentialityId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }

        [HttpPut("{id}/essentiality/{essentialityId}", Name = "UpdateEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEssentiality(Guid id, Guid essentialityId, UpdateEssentialityCommand command)
        {
            try
            {
                command.Id = id;
                command.EssentialityId = essentialityId;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Essentiality updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateEssentiality: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateEssentiality: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the gene essentiality";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }

        [HttpDelete("{id}/essentiality/{essentialityId}", Name = "DeleteEssentiality")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEssentiality(Guid id, Guid essentialityId)
        {
            try
            {
                await _mediator.Send(new DeleteEssentialityCommand { Id = id, GeneId = id, EssentialityId = essentialityId });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Gene Essentiality deleted successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteEssentiality: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the gene essentiality";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }


    }
}