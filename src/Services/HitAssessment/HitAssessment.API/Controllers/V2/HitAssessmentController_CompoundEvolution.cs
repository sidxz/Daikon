
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using HitAssessment.Application.Features.Commands.DeleteHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.NewHaCompoundEvolution;
using HitAssessment.Application.Features.Commands.UpdateHaCompoundEvolution;
using Microsoft.AspNetCore.Mvc;

namespace HitAssessment.API.Controllers.V2
{
    public partial class HitAssessmentController : ControllerBase
    {
        [HttpPost("{haId}/compound-evolution/" , Name = "AddCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddCompoundEvolution(Guid haId, NewHaCompoundEvolutionCommand command)
        {
            command.Id = haId;
            var compoundEvolutionId = Guid.NewGuid();
            try
            {
                command.CompoundEvolutionId = compoundEvolutionId;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = compoundEvolutionId,
                    Message = "HA Compound Evolution added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddCompoundEvolution: ArgumentNullException {Id}", compoundEvolutionId);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddCompoundEvolution: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the Compound Evolution";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = compoundEvolutionId,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }



        [HttpPut("{haId}/compound-evolution/{id}", Name = "UpdateCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCompoundEvolution(Guid haId, Guid id, UpdateHaCompoundEvolutionCommand command)
        {

            try
            {
                command.Id = id;
                command.CompoundEvolutionId = id;
                
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "HA Compound Evolution updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateCompoundEvolution: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateCompoundEvolution: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the Compound Evolution";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }



        [HttpDelete("{haId}/compound-evolution/{id}", Name = "DeleteCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCompoundEvolution(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteHaCompoundEvolutionCommand { CompoundEvolutionId = id });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "HA Compound Evolution deleted successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteHitAssessment: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the screen";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }
    }
}