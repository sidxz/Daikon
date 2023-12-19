
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Screen.Application.Features.Commands.DeleteHitCollection;
using Screen.Application.Features.Commands.NewHitCollection;
using Screen.Application.Features.Commands.UpdateHitCollection;

namespace Screen.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class HitCollectionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HitCollectionController> _logger;

        public HitCollectionController(IMediator mediator, ILogger<HitCollectionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost(Name = "AddHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<IActionResult> AddHitCollection(NewHitCollectionCommand command)
        {
            var id = Guid.NewGuid();
            try
            { 
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Hit collection created successfully",

                });
                
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddHitCollection: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddHitCollection: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the hit collection";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPut("{id}", Name = "UpdateHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> UpdateHitCollection (Guid id, UpdateHitCollectionCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);
                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Hit Collection updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateHitCollection: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateHitCollection: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the hit collection";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpDelete("{id}", Name = "DeleteHitCollection")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHitCollection(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteHitCollectionCommand { Id = id });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Hit Collection deleted successfully",
                });
            }
    
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteHitCollection: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the hit collection";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }
    }
}