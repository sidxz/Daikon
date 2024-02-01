
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserStore.Application.Features.Commands.APIResources.AddAPIResource;
using UserStore.Application.Features.Commands.APIResources.DeleteAPIResource;
using UserStore.Application.Features.Commands.APIResources.UpdateAPIResource;
using UserStore.Application.Features.Queries.APIResources.ListAPIResources;

namespace UserStore.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class APIResourcesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<APIResourcesController> _logger;

        public APIResourcesController(IMediator mediator, ILogger<APIResourcesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // List API Resources
        [HttpGet]
        [ProducesResponseType(typeof(List<APIResourceVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListAPIResources()
        {
            try
            {
                var response = await _mediator.Send(new ListAPIResourcesQuery());
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid request.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving API resources";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        // Add API Resource
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAPIResource([FromBody] AddAPIResourceCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "APIResource added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("APIResource ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("APIResource: Requested Resource Already Exists {Name}", ex.Message);
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

        // Update API Resource
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAPIResource(Guid id, [FromBody] UpdateAPIResourceCommand command)
        {
            command.Id = id;
            try
            {
                await _mediator.Send(command);
                return Ok(new BaseResponse
                {
                    Message = "APIResource updated successfully"
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("APIResource ArgumentNullException {Id}", command.Id);
                return BadRequest(new BaseResponse
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the APIResource";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        // Delete API Resource
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAPIResource(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteAPIResourceCommand { Id = id });
                return Ok(new BaseResponse
                {
                    Message = "APIResource deleted successfully"
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("DeleteAPIResource ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the APIResource";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}