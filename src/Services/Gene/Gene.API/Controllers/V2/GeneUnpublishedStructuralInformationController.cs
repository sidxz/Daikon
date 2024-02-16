
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteUnpublishedStructuralInformation;
using Gene.Application.Features.Command.NewUnpublishedStructuralInformation;
using Gene.Application.Features.Command.UpdateUnpublishedStructuralInformation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {
   

        [HttpPost("{id}/unpublished-structural-information", Name = "AddUnpublishedStructuralInformation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddUnpublishedStructuralInformation(Guid id, NewUnpublishedStructuralInformationCommand command)
        {
            var unpublishedStructuralInformationId = Guid.NewGuid();
            try
            {
                command.UnpublishedStructuralInformationId = unpublishedStructuralInformationId;
                command.Id = id;
                command.GeneId = id;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = unpublishedStructuralInformationId,
                    Message = "UnpublishedStructuralInformation added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddUnpublishedStructuralInformation: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the unpublishedStructuralInformation";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = unpublishedStructuralInformationId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }




        [HttpPut("{id}/unpublished-structural-information/{unpublishedStructuralInformationId}", Name = "UpdateUnpublishedStructuralInformation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateUnpublishedStructuralInformation(Guid id, Guid unpublishedStructuralInformationId, UpdateUnpublishedStructuralInformationCommand command)
        {
            try
            {
                command.Id = id;
                command.UnpublishedStructuralInformationId = unpublishedStructuralInformationId;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "UnpublishedStructuralInformation updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateUnpublishedStructuralInformation: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateUnpublishedStructuralInformation: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the gene unpublishedStructuralInformation";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }

        [HttpDelete("{id}/unpublished-structural-information/{unpublishedStructuralInformationId}", Name = "DeleteUnpublishedStructuralInformation")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUnpublishedStructuralInformation(Guid id, Guid unpublishedStructuralInformationId)
        {
            try
            {
                await _mediator.Send(new DeleteUnpublishedStructuralInformationCommand { Id = id, GeneId = id, UnpublishedStructuralInformationId = unpublishedStructuralInformationId });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Gene UnpublishedStructuralInformation deleted successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteUnpublishedStructuralInformation: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the gene unpublishedStructuralInformation";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }


    }
}