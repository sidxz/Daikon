
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;

using Gene.Application.Features.Command.DeleteStrain;
using Gene.Application.Features.Command.NewStrain;
using Gene.Application.Features.Command.UpdateStrain;
using Gene.Application.Features.Queries.GetStrain;
using Gene.Application.Features.Queries.GetStrain.ById;
using Gene.Application.Features.Queries.GetStrain.ByName;
using Gene.Application.Features.Queries.GetStrainsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class StrainController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StrainController> _logger;

        public StrainController(IMediator mediator, ILogger<StrainController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetStrainsList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<StrainsListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<StrainsListVM>>> GetStrainsList()
        {
            try
            {
                var strainsList = await _mediator.Send(new GetStrainsListQuery());
                return Ok(strainsList);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the strains list";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }




        [HttpGet("{id}", Name = "GetStrainDefault")]
        [HttpGet("by-id/{id}", Name = "GetStrainById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(StrainVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StrainVM>> GetStrainById(Guid id)
        {
            try
            {
                var strain = await _mediator.Send(new GetStrainByIdQuery { Id = id});
                return Ok(strain);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetStrainById: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the strain";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpGet("by-name/{name}", Name = "GetStrainByName")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(StrainVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StrainVM>> GetStrainByName(string name)
        {
            try
            {
                var strain = await _mediator.Send(new GetStrainByNameQuery { Name = name });
                return Ok(strain);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetStrainByName: Requested Resource Not Found {name}", name);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the strain";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpPost(Name = "AddStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddStrain(NewStrainCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Strain added successfully",
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddStrain: Requested Resource Already Exists {Name}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the strain";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }



        [HttpPut("{id}", Name = "UpdateStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateStrain(Guid id, UpdateStrainCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Strain updated successfully",
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateStrain: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the strain";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }


        [HttpDelete("{id}", Name = "DeleteStrain")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteStrain(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteStrainCommand { Id = id });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Strain deleted successfully",
                });
            }

            catch (ResourceCannotBeDeletedException ex)
            {
                _logger.LogInformation("DeleteStrain: Requested Resource Cannot Be Deleted {Id}", id);
                return Conflict(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("DeleteGene: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while deleting the gene";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }
    }
}