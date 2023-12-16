
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Commands.DeleteScreen;
using Screen.Application.Features.Commands.NewScreen;
using Screen.Application.Features.Commands.UpdateScreen;
using Screen.Application.Features.Queries.GetScreen.ById;

namespace Screen.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class ScreenController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ScreenController> _logger;

        public ScreenController(IMediator mediator, ILogger<ScreenController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        // [HttpGet("{id}", Name = "GetScreenDefault")]
        // [HttpGet("by-id/{id}", Name = "GetScreenById")]
        // [MapToApiVersion("2.0")]
        // [ProducesResponseType(typeof(ScreenVM), (int)HttpStatusCode.OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<ActionResult<ScreenVM>> GetScreenById(Guid id, [FromQuery] bool WithMeta = false)
        // {
        //     try
        //     {
        //         var screen = await _mediator.Send(new GetScreenByIdQuery { Id = id, WithMeta = WithMeta });
        //         return Ok(screen);
        //     }
        //     catch (ResourceNotFoundException ex)
        //     {
        //         _logger.LogInformation("GetScreenById: Requested Resource Not Found {Id}", id);
        //         return NotFound(new BaseResponse
        //         {
        //             Message = ex.Message
        //         });
        //     }

        //     catch (InvalidOperationException ex)
        //     {
        //         _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
        //         return BadRequest(new BaseResponse
        //         {
        //             Message = ex.Message
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the screen";
        //         _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

        //         return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        //         {
        //             Message = SAFE_ERROR_MESSAGE
        //         });
        //     }
        // }




        [HttpPost(Name = "AddScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddScreen(NewScreenCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Screen added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddScreen: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddScreen: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the screen";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }



        [HttpPut("{id}", Name = "UpdateScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateScreen(Guid id, UpdateScreenCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Screen updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateScreen: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateScreen: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the screen";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }


        // [HttpPut("{id}/update-associated-targets", Name = "UpdateScreenAssociatedTargets")]
        // [MapToApiVersion("2.0")]
        // [ProducesResponseType((int)HttpStatusCode.OK)]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // public async Task<ActionResult> UpdateScreenAssociatedTargets(Guid id, UpdateScreenAssociatedTargetsCommand command)
        // {
        //     try
        //     {
        //         command.Id = id;
        //         await _mediator.Send(command);

        //         return StatusCode(StatusCodes.Status200OK, new BaseResponse
        //         {
        //             Message = "Screen updated successfully",
        //         });
        //     }
        //     catch (ArgumentNullException ex)
        //     {
        //         _logger.LogInformation("UpdateScreen: ArgumentNullException {Id}", id);
        //         return BadRequest(new BaseResponse
        //         {
        //             Message = ex.Message
        //         });
        //     }

        //     catch (ResourceNotFoundException ex)
        //     {
        //         _logger.LogInformation("UpdateScreen: Requested Resource Not Found {Id}", id);
        //         return NotFound(new BaseResponse
        //         {
        //             Message = ex.Message
        //         });
        //     }
        //     catch (InvalidOperationException ex)
        //     {
        //         _logger.Log(LogLevel.Warning, ex, "Client Made a bad request");
        //         return BadRequest(new BaseResponse
        //         {
        //             Message = ex.Message
        //         });
        //     }

        //     catch (Exception ex)
        //     {
        //         const string SAFE_ERROR_MESSAGE = "An error occurred while updating the screen";
        //         _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

        //         return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        //         {
        //             Message = SAFE_ERROR_MESSAGE
        //         });
        //     }

        // }

        [HttpDelete("{id}", Name = "DeleteScreen")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteScreen(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteScreenCommand { Id = id });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Screen deleted successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteScreen: Requested Resource Not Found {Id}", id);
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