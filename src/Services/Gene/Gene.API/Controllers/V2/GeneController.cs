
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.DeleteGene;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.UpdateGene;
using Gene.Application.Features.Queries.GetGene;
using Gene.Application.Features.Queries.GetGene.ByAccession;
using Gene.Application.Features.Queries.GetGene.ById;
using Gene.Application.Features.Queries.GetGenesList;
using Gene.Application.Features.Queries.GetGeneVersions;
using Gene.Application.Features.Queries.GetGeneVersions.ById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]

    public class GeneController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<GeneController> _logger;

        public GeneController(IMediator mediator, ILogger<GeneController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetGenesList")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<GenesListVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<GenesListVM>>> GetGenesList()
        {
            try
            {
                var genesList = await _mediator.Send(new GetGenesListQuery());
                return Ok(genesList);
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the genes list";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("{id}", Name = "GetGeneDefault")]
        [HttpGet("by-id/{id}", Name = "GetGeneById")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(GeneVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GeneVM>> GetGeneById(Guid id, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var gene = await _mediator.Send(new GetGeneByIdQuery { Id = id, WithMeta = WithMeta });
                return Ok(gene);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetGeneById: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the gene";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpGet("by-accession/{accessionNo}", Name = "GetGeneByAccession")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(GeneVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GeneVM>> GetGeneByAccession(string accessionNo, [FromQuery] bool WithMeta = false)
        {
            try
            {
                var gene = await _mediator.Send(new GetGeneByAccessionQuery { AccessionNumber = accessionNo, WithMeta = WithMeta });
                return Ok(gene);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetGeneById: Requested Resource Not Found {accession}", accessionNo);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the gene";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }


        [HttpPost(Name = "AddGene")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddGene(NewGeneCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new AddResponse
                {
                    Id = id,
                    Message = "Gene added successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateGene: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (DuplicateEntityRequestException ex)
            {
                _logger.LogInformation("AddGene: Requested Resource Already Exists {accessionNo}", ex.Message);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while adding the gene";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new AddResponse
                {
                    Id = id,
                    Message = SAFE_ERROR_MESSAGE
                });
            }


        }

        [HttpPut("{id}", Name = "UpdateGene")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateGene(Guid id, UpdateGeneCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Gene updated successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("UpdateGene: ArgumentNullException {Id}", id);
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }

            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("UpdateGene: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while updating the gene";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }

        [HttpDelete("{id}", Name = "DeleteGene")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteGene(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteGeneCommand { Id = id });

                return StatusCode(StatusCodes.Status200OK, new BaseResponse
                {
                    Message = "Gene deleted successfully",
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


        [HttpGet("revision/{id}", Name = "GetGeneRevision")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(GeneVersionsVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GeneVM>> GetGeneRevision(Guid id)
        {
            try
            {
                var geneRevision = await _mediator.Send(new GetGeneVersionsQuery { Id = id });
                return Ok(geneRevision);
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogInformation("GetGeneRevision: Requested Resource Not Found {Id}", id);
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
                const string SAFE_ERROR_MESSAGE = "An error occurred while retrieving the gene";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}