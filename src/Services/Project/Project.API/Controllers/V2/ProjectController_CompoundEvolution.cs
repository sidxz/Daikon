
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Project.Application.Features.Commands.DeleteProjectCompoundEvolution;
using Project.Application.Features.Commands.NewProjectCompoundEvolution;
using Project.Application.Features.Commands.UpdateProjectCompoundEvolution;
using Microsoft.AspNetCore.Mvc;

namespace Project.API.Controllers.V2
{
    public partial class ProjectController : ControllerBase
    {
        [HttpPost("{projectId}/compound-evolution/", Name = "AddCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddCompoundEvolution(Guid projectId, NewProjectCompoundEvolutionCommand command)
        {
            command.Id = projectId;
            var compoundEvolutionId = Guid.NewGuid();
            try
            {
                command.CompoundEvolutionId = compoundEvolutionId;
                var response = await _mediator.Send(command);
                return StatusCode(StatusCodes.Status201Created, response);

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



        [HttpPut("{projectId}/compound-evolution/{id}", Name = "UpdateCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCompoundEvolution(Guid projectId, Guid id, UpdateProjectCompoundEvolutionCommand command)
        {

            try
            {
                command.Id = projectId;
                command.CompoundEvolutionId = id;

                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Project Compound Evolution updated successfully",
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



        [HttpDelete("{projectId}/compound-evolution/{id}", Name = "DeleteCompoundEvolution")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCompoundEvolution(Guid projectId, Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteProjectCompoundEvolutionCommand
                {
                    Id = projectId,
                    CompoundEvolutionId = id
                });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Project Compound Evolution deleted successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteProject: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the Compound Evolution";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }
    }
}