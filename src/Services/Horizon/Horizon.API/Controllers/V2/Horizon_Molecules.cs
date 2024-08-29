using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Horizon.Application.Features.Queries.CompoundRelations;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.API.Controllers.V2
{
    public partial class Horizon
    {
        [HttpGet("find-molecule-relations/{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> FindMoleculeRelations(Guid id)
        {
            try
            {
                var relations = await _mediator.Send(new CompoundRelationsQuery { Id = id });

                return Ok(relations);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("FindMoleculeRelations: ArgumentNullException {id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("FindMoleculeRelations: Requested Resource Not Found {id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while finding the related target";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}