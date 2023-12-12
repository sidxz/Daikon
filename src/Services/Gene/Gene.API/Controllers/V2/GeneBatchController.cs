
using System.Net;
using CQRS.Core.Exceptions;
using CQRS.Core.Responses;
using Gene.Application.Features.Command.BatchImportOne;
using Gene.Application.Features.Queries.BatchExportAll;
using Gene.Application.Features.Queries.GetGene;
using Gene.Application.Features.Queries.GetGene.ById;
using Gene.Domain.Batch;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gene.API.Controllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class GeneBatchController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GeneController> _logger;

        public GeneBatchController(IMediator mediator, ILogger<GeneController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet("{id}", Name = "GetOne")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(GeneVM), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GeneVM>> GetOne(Guid id, [FromQuery] bool WithMeta = false)
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





        [HttpGet(Name = "GetAll")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<GeneExport>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<GeneExport>>> GetAll()
        {
            try
            {
                var geneExportList = await _mediator.Send(new BatchExportAllQuery { });
                return Ok(geneExportList);
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

        [HttpPost(Name = "ImportOne")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(Unit), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Unit>> ImportOne([FromBody] BatchImportOneCommand command)
        {
            try
            {
                await _mediator.Send(command);

                return StatusCode(StatusCodes.Status201Created, new BaseResponse
                {
                    Message = "Gene imported successfully",
                });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("ImportOne: ArgumentNullException");
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

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }

        }
    }
}