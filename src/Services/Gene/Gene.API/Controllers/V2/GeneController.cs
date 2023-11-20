using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gene.API.DTOs;
using Gene.Application.Features.Command.NewGene;
using Gene.Application.Features.Command.UpdateGene;
using Gene.Application.Features.Queries.GetGene;
using Gene.Application.Features.Queries.GetGene.ById;
using Gene.Application.Features.Queries.GetGenesList;
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
        public async Task<ActionResult<GeneVM>> GetGeneById(Guid id)
        {
            try
            {
                var gene = await _mediator.Send(new GetGeneByIdQuery { Id = id });
                return Ok(gene);
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

                return StatusCode(StatusCodes.Status201Created, new AddGeneResponse
                {
                    Id = id,
                    Message = "Gene added successfully",
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

                return StatusCode(StatusCodes.Status500InternalServerError, new AddGeneResponse
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
    }
}