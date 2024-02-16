
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteProteinProduction;
using Gene.Application.Features.Command.NewProteinProduction;
using Gene.Application.Features.Command.UpdateProteinProduction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{

    public partial class GeneController : ControllerBase
    {
     
        [HttpPost("{id}/protein-production", Name = "AddProteinProduction")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddProteinProduction(Guid id, NewProteinProductionCommand command)
        {
            var proteinProductionId = Guid.NewGuid();
            try
            {
                command.ProteinProductionId = proteinProductionId;
                command.Id = id;
                command.GeneId = id;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = proteinProductionId,
                    Message = "Protein Production added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddProteinProduction: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the protein production";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = proteinProductionId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }




        [HttpPut("{id}/protein-production/{proteinProductionId}", Name = "UpdateProteinProduction")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProteinProduction(Guid id, Guid proteinProductionId, UpdateProteinProductionCommand command)
        {
            try
            {
                command.Id = id;
                command.ProteinProductionId = proteinProductionId;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Protein Production updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateProteinProduction: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateProteinProduction: Requested Resource Not Found {Id}", id);
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

        [HttpDelete("{id}/protein-production/{proteinProductionId}", Name = "DeleteProteinProduction")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProteinProduction(Guid id, Guid proteinProductionId)
        {
            try
            {
                await _mediator.Send(new DeleteProteinProductionCommand { Id = id, GeneId = id, ProteinProductionId = proteinProductionId });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Gene Protein Production deleted successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteProteinProduction: Requested Resource Not Found {Id}", id);
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