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
using Target.Application.Features.Queries.GetToxicology.ByTargetId;
using Target.Application.Features.Queries.GetToxicologyList;

namespace Target.API.Controllers.V2
{
    public partial class TargetController : ControllerBase
    {
        [HttpGet("{id}/toxicology", Name = "GetToxicologyByTarget")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetToxicologyByTarget(Guid id, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var toxicologies = await _mediator.Send(new GetToxicologyByTargetQuery { TargetId = id, WithMeta = WithMeta });

                return Ok(toxicologies);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching the toxicology";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpGet("toxicology", Name = "GetToxicologyList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetToxicologyList([FromQuery] bool WithMeta = false)
        {
            try
            {
                var toxicologies = await _mediator.Send(new GetToxicologyListQuery { WithMeta = WithMeta });

                return Ok(toxicologies);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while fetching the toxicology";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

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

        [HttpPost("toxicology/batch-add-update-toxicity", Name = "BatchAddOrUpdateToxicology")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> BatchAddOrUpdateToxicology(IEnumerable<AddOrUpdateToxicologyCommand> commands)
        {
            var resp = new BatchResponse
            {
                Success = new List<Guid>(),
                Failed = new List<Guid>()
            };
            try
            {
                foreach (var command in commands)
                {
                    try
                    {
                        await _mediator.Send(command);
                        resp.Success.Add(command.TargetId);
                    }
                    catch (Exception ex)
                    {
                        resp.Failed.Add(command.TargetId);
                    }
                }

                return Ok(resp);
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