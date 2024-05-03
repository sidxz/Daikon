
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HitAssessment.Application.Features.Commands.DeleteHitAssessment;
using HitAssessment.Application.Features.Commands.NewHitAssessment;
using HitAssessment.Application.Features.Commands.UpdateHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessment.ById;
using HitAssessment.Application.Features.Queries.GetHitAssessment;
using HitAssessment.Application.Features.Queries.GetHitAssessmentList;
using HitAssessment.Application.Features.Queries.GetHitAssessment.GetHitAssessmentList;
using System.Text.Json;
using HitAssessment.Application.Features.Batch.ImportOne;

namespace HitAssessment.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public partial class HitAssessmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HitAssessmentController> _logger;

        public HitAssessmentController(IMediator mediator, ILogger<HitAssessmentController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetHitAssessmentList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(IEnumerable<HitAssessmentListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<HitAssessmentListVM>>> GetHitAssessmentList([FromQuery] bool WithMeta = false)
        {
            try
            {
                var screens = await _mediator.Send(new GetHitAssessmentListQuery { WithMeta = WithMeta });
                return Ok(screens);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the hit assessment list";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpGet("{id}", Name = "GetHitAssessmentDefault")]
        [HttpGet("by-id/{id}", Name = "GetHitAssessmentById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(HitAssessmentVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HitAssessmentVM>> GetHitAssessmentById(Guid id, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var screen = await _mediator.Send(new GetHitAssessmentByIdQuery { Id = id, WithMeta = WithMeta });
                return Ok(screen);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetHitAssessmentById: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the hit assessment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }




        [HttpPost(Name = "AddHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddHitAssessment([FromBody] NewHitAssessmentCommand command)
        {

            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "HitAssessment added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("AddHitAssessment: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddHitAssessment: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the hit assessment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }



        [HttpPut("{id}", Name = "UpdateHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateHitAssessment(Guid id, UpdateHitAssessmentCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "HitAssessment updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateHitAssessment: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateHitAssessment: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the hit assessment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }



        [HttpDelete("{id}", Name = "DeleteHitAssessment")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHitAssessment(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteHitAssessmentCommand { Id = id });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "HitAssessment deleted successfully",
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the hit assessment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }


        [HttpPost("import-one", Name = "import-one")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> ImportHitAssessment([FromBody] ImportOneCommand command)
        {

            var id = Guid.NewGuid();
            try
            {
                command.Id = command.Id == Guid.Empty ? id : command.Id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "HitAssessment imported successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("ImportHitAssessment: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("ImportHitAssessment: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while importing the hit assessment";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }
    }
}